﻿#region

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using Confuser.Core;
using Confuser.Core.Project;
using dnlib.DotNet;
using ILogger = Confuser.Core.ILogger;

#endregion

namespace KoiVM.Confuser.Internal
{
    [Obfuscation(Exclude = false, Feature = "+koi;")]
    public class SavePhase : ProtectionPhase
    {
        public SavePhase(Protection parent)
            : base(parent)
        {
        }

        public override ProtectionTargets Targets => ProtectionTargets.Modules;

        public override string Name => "Save runtime library";

        private string ProtectRT(ConfuserContext context, string fileName)
        {
            string outDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outDir);

            var proj = new ConfuserProject
            {
                Seed = context.Project.Seed,
                Debug = context.Project.Debug
            };
            //foreach (Rule rule in context.Project.Rules) {
            //    var r = rule.Clone();
            //    r.RemoveWhere(prot => prot.Id == Parent.Id);
            //    proj.Rules.Add(r);
            //}
            proj.Rules.Add(new Rule
            {
                new SettingItem<Protection>("anti ildasm"),
                new SettingItem<Protection>("ref proxy")
                {
                    {"mode", "mild"},
                    {"typeErasure", "true"}
                },
                new SettingItem<Protection>("rename")
                {
                    {"mode", "repeating"}
                }
            });
            proj.Add(new ProjectModule
            {
                Path = fileName
            });
            proj.BaseDirectory = Path.GetDirectoryName(fileName);
            proj.OutputDirectory = outDir;
            foreach(string path in context.Project.ProbePaths)
                proj.ProbePaths.Add(path);
            proj.ProbePaths.Add(context.Project.BaseDirectory);

            StrongNameKey snKey = null;
            foreach(ModuleDefMD module in context.Modules)
            {
                snKey = context.Annotations.Get<StrongNameKey>(module, Marker.SNKey);
                if(snKey != null)
                    break;
            }

            try
            {
                ConfuserEngine.Run(new ConfuserParameters
                {
                    Logger = new RTLogger(context.Logger),
                    Marker = new RTMarker(snKey),
                    Project = proj
                }, context.CancellationToken).Wait();
            }
            catch(AggregateException ex)
            {
                context.Logger.Error("Failed to protect Runtime.");
                throw new ConfuserException(ex);
            }

            return Path.Combine(outDir, Path.GetFileName(fileName));
        }

        protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
        {
            Virtualizer vr = context.Annotations.Get<Virtualizer>(context, Fish.VirtualizerKey);
            ModuleDef merge = context.Annotations.Get<ModuleDef>(context, Fish.MergeKey);

            if(merge != null)
                return;

            string tmpDir = Path.GetTempPath();
            string outDir = Path.Combine(tmpDir, Path.GetRandomFileName());
            Directory.CreateDirectory(outDir);

            string rtPath = vr.SaveRuntime(outDir);
            if(context.Packer != null)
            {
#if DEBUG
                string protRtPath = rtPath;
#else
				var protRtPath = ProtectRT(context, rtPath);
#endif
                context.ExternalModules.Add(File.ReadAllBytes(protRtPath));
                foreach(Rule rule in context.Project.Rules)
                    rule.RemoveWhere(item => item.Id == this.Parent.Id);
            }
            else
            {
                outDir = Path.GetDirectoryName(rtPath);
                foreach(string file in Directory.GetFiles(outDir))
                {
                    string path = Path.Combine(context.OutputDirectory, Path.GetFileName(file));
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.Copy(file, path, true);
                }
            }
        }

        internal class RTMarker : Marker
        {
            private readonly StrongNameKey snKey;

            public RTMarker(StrongNameKey snKey) => this.snKey = snKey;

            protected override MarkerResult MarkProject(ConfuserProject proj, ConfuserContext context)
            {
                MarkerResult result = base.MarkProject(proj, context);
                foreach(ModuleDefMD module in result.Modules)
                    context.Annotations.Set(module, SNKey, this.snKey);
                return result;
            }
        }

        internal class RTLogger : ILogger
        {
            private readonly ILogger baseLogger;

            public RTLogger(ILogger baseLogger) => this.baseLogger = baseLogger;

            public static void Input(string text)
            {
                Colorful.Console.Write($"                         [{DateTime.Now}] ", Color.DarkRed);
                Colorful.Console.Write($"{text}\n", Color.White);
            }

            public void Log(string msg) => Input(msg);

            public void LogFormat(string format, params object[] args) => Input(string.Format(format, args));

            public void Error(string msg) => Input(msg);

            public void ErrorException(string msg, Exception ex)
            {
                Input($"ERROR: {msg}");
                Input($"Exception: {ex}");
            }

            public void ErrorFormat(string format, params object[] args) => Input($"ERROR: {string.Format(format, args)}");

            public void Progress(int progress, int overall)
            {
            }

            public void EndProgress()
            {
            }

            public void Finish(bool successful)
            {
                if(!successful)
                    throw new ConfuserException(null);
                this.baseLogger.Log("Finish protecting Runtime.");
            }
        }
    }
}
﻿#region

using System;
using dnlib.DotNet;
using KoiVM.AST.IR;

#endregion

namespace KoiVM.VMIR.Transforms
{
    public class MetadataTransform : ITransform
    {
        public void Initialize(IRTransformer tr)
        {
        }

        public void Transform(IRTransformer tr) => tr.Instructions.VisitInstrs(this.VisitInstr, tr);

        private void VisitInstr(IRInstrList instrs, IRInstruction instr, ref int index, IRTransformer tr)
        {
            instr.Operand1 = this.TransformMD(instr.Operand1, tr);
            instr.Operand2 = this.TransformMD(instr.Operand2, tr);
        }

        private IIROperand TransformMD(IIROperand operand, IRTransformer tr)
        {
            if (operand is IRMetaTarget target)
            {
                if (!target.LateResolve)
                {
                    if (!(target.MetadataItem is IMemberRef))
                        throw new NotSupportedException();
                    return IRConstant.FromI4((int)tr.VM.Data.GetId((IMemberRef)target.MetadataItem));
                }
            }
            return operand;
        }
    }
}
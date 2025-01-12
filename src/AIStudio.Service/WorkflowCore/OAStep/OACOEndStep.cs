﻿using Coldairarrow.Util;
using System;
using System.Linq;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Primitives;

namespace AIStudio.Service.WorkflowCore
{
    /// <summary>
    /// 并行结束
    /// </summary>
    public class OACOEndStep : OABaseStep , IEndStep
    {
        /// <summary>
        /// 节点触发
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            OAData oAData = GetStep(context);

            //改变流程图颜色
            var node = oAData.nodes.FirstOrDefault(p => p.id == OAStep.Id);
            if (node != null)
            {
                node.color = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.Orange);
            }

            if (OAStep.PreStepId != null)
            {
                var def = _registry.GetDefinition(context.Workflow.WorkflowDefinitionId, context.Workflow.Version);
                foreach (var id in OAStep.PreStepId)
                {                   
                    var pre = def.Steps.Find(p => p.ExternalId == id);
                    if (!context.Workflow.ExecutionPointers.Any(p => p.StepId == pre.Id && p.Status == PointerStatus.Complete))
                    {
                        var result = ExecutionResult.Persist(context.PersistenceData);
                        result.SleepFor = TimeSpan.FromSeconds(5);
                        return result;
                    }
                }
            }

            OAStep.Status = 100;

            //改变流程图颜色
            if (node != null)
            {
                node.color = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.LightGreen);
            }           

            return ExecutionResult.Next();
        }
    }
}

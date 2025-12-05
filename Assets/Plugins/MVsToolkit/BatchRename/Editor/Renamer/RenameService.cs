using System.Collections.Generic;
using System.Linq;

namespace MVsToolkit.BatchRename
{
    public class RenameService : IRenamer
    {
        public IEnumerable<RenameResult> Preview(IEnumerable<IRenameTarget> targets, RenameConfig config)
        {
            IEnumerable<IRenameTarget> filteredTargets = targets.Where(t => config.Rules.All(r => r.Matches(t, null)));
            List<RenameResult> results = new List<RenameResult>(capacity: filteredTargets.Count());
            
            int index = 0;
            int total = filteredTargets.Count();

            foreach (var target in filteredTargets)
            {
                var ctx = new RenameContext
                {
                    GlobalIndex = index,
                    TotalCount = total,
                    AssetPath = target.Path,
                    IsAsset = target.UnityObject as UnityEngine.GameObject == null,
                    TargetObject = target.UnityObject
                };
                
                string newName = target.Name;
                foreach (var operation in config.Operations)
                {
                    newName = operation.Apply(newName, ctx);
                }
                
                results.Add(new RenameResult
                {
                    Target = target,
                    OldName = target.Name,
                    NewName = newName,
                    HasConflict = false,
                });
                
                index++;
            }
            
            return results;
        }

        public void Apply(IEnumerable<RenameResult> results, RenameConfig config)
        {
            foreach (var result in results)
            {
                result.Target.SetName(result.NewName);
            }
            
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace MVsToolkit.BatchRename
{
    public class RenamerService : IRenamer
    {
        public IEnumerable<RenameResult> Preview(IEnumerable<IRenameTarget> targets, RenameConfig config)
        {
            IEnumerable<IRenameTarget> filteredTargets = targets.Where(t => config.Rules.All(r => r?.Matches(t, null) ?? true));
            IEnumerable<IRenameTarget> renameTargets = filteredTargets.ToArray();
            List<RenameResult> results = new List<RenameResult>(capacity: renameTargets.Count());
            
            int index = 0;
            int total = renameTargets.Count();

            foreach (IRenameTarget target in renameTargets)
            {
                RenameContext ctx = new()
                {
                    GlobalIndex = index,
                    TotalCount = total,
                    AssetPath = target.Path,
                    IsAsset = !(target.UnityObject as UnityEngine.GameObject),
                    TargetObject = target.UnityObject
                };

                string newName = target.Name;
                foreach (IRenameOperation operation in config.Operations)
                    newName = operation?.Apply(newName, ctx);

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
            foreach (RenameResult result in results)
            {
                result.Target.SetName(result.NewName);
            }
            
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}
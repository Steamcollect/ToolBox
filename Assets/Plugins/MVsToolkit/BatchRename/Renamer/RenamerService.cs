using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MVsToolkit.BatchRename
{
    public class RenamerService : IRenamer
    {
        public IEnumerable<RenameResult> Preview(IEnumerable<IRenameTarget> targets, RenameConfig config)
        {
            if (targets == null || config == null)
                return new List<RenameResult>();

            IEnumerable<IRenameTarget> filteredTargets =
                targets.Where(t => t.IsValidTarget() && config.Rules.All(r => r?.Matches(t, null) ?? true));
            IEnumerable<IRenameTarget> renameTargets = filteredTargets.ToArray();
            var results = new List<RenameResult>(renameTargets.Count());

            int index = 0;
            int total = renameTargets.Count();

            foreach (IRenameTarget target in renameTargets)
            {
                // Guard: skip null targets
                if (target == null)
                {
                    index++;
                    continue;
                }

                RenameContext ctx = new()
                {
                    GlobalIndex = index,
                    TotalCount = total,
                    AssetPath = target.Path,
                    IsAsset = !(target.UnityObject as GameObject),
                    TargetObject = target.UnityObject
                };

                if (!ctx.Validate())
                {
                    Debug.LogWarning($"[BatchRename] Invalid rename context for target: {target.Name}");
                    index++;
                    continue;
                }

                StringBuilder newNameBuilder = new(target.Name);

                if (config.Operations != null)
                    foreach (IRenameOperation operation in config.Operations)
                    {
                        if (operation == null)
                            continue;

                        try
                        {
                            operation.Apply(newNameBuilder, ctx);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning(
                                $"[BatchRename] Operation {operation.GetType().Name} failed for '{target.Name}': {ex.Message}");
                        }
                    }

                string finalName = newNameBuilder.ToString();

                results.Add(new RenameResult
                {
                    Target = target,
                    OldName = target.Name,
                    NewName = finalName,
                    HasConflict = false
                });

                index++;
            }

            return results;
        }

        public void Apply(IEnumerable<RenameResult> results, RenameConfig config)
        {
            if (results == null)
                return;
            try
            {
                IEnumerable<RenameResult> renameResults = results as RenameResult[] ?? results.ToArray();
                Undo.RecordObjects(renameResults.Select(o => o.Target.UnityObject).ToArray(), "Batch Rename Apply");

                foreach (RenameResult result in renameResults) result?.Target?.SetName(result.NewName);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BatchRename] Error during apply phase: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
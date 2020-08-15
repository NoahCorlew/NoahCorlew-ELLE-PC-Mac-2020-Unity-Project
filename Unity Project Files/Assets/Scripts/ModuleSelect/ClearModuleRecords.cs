using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Debug = UnityEngine.Debug;
using System.Reflection;

public class ClearModuleRecords : MonoBehaviour
{
    public void clearSavedUserInfo()
    {
        string moduleName = CollectionManager.moduleCollection[ModuleSelect.currentModuleID].getName();

        File.Delete(Application.persistentDataPath + "/UserRecords/Module" + ModuleSelect.currentModuleID + ".json");

        ModuleInfo newMod = new ModuleInfo(ModuleSelect.currentModuleID, moduleName, 0, 0, 0, 0, 0, 0);

        CollectionManager.moduleCollection.Remove(ModuleSelect.currentModuleID);
        CollectionManager.moduleCollection.Add(ModuleSelect.currentModuleID, newMod);

        string moduleJson = newMod.createJson();
        newMod.saveJson();

        ModuleSelect.setSelectedModule(ModuleSelect.currentModuleID);
    }
}

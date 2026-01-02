using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.installers
{
  public static class PackageInstaller
  {
    private static AddRequest request;
    private static List<string> packagesToInstall;
    private static ListRequest p_listRequest;
    private static int currentIndex;

    [MenuItem("Tools/Install Required Packages")]
    public static void CheckInstalledPackages()
    {
      packagesToInstall = new List<string>
      {
        "com.unity.mathematics",
        "com.unity.collections",
        "com.unity.textmeshpro",
        "com.unity.ugui",
        "com.unity.visualscripting",
      };

      Debug.Log("[Stationeers Modding Tools] | Checking for missing packages");
      RetrievePackageList(packagesToInstall);
    }
    private static void RetrievePackageList(List<string> packages)
    {
      p_listRequest = Client.List();
      EditorApplication.update += MonitorListRequest;

      void MonitorListRequest()
      {
        if (p_listRequest.IsCompleted)
        {
          EditorApplication.update -= MonitorListRequest;

          if (p_listRequest.Status == StatusCode.Success)
          {
            var installed = new HashSet<string>(p_listRequest.Result.Select(p => p.name));
            packagesToInstall = packages.Where(pkg => !installed.Contains(pkg)).ToList();

            if (packagesToInstall.Count == 0)
            {
              EditorUtility.DisplayDialog("[Stationeers Modding Tools]", "All required packages are already installed", "Close");
            }
            else
            {
              Debug.Log("[Stationeers Modding Tools] | Missing packages: " + string.Join(", ", packagesToInstall));
              InstallPackages();
            }
          }
          else
          {
            EditorUtility.DisplayDialog("[Stationeers Modding Tools]", "Encountered Error, check console logs", "Close");
            Debug.LogError($"Error retrieving packages in project: {p_listRequest.Error}");
          }
          p_listRequest = null;
        }
      }
    }
    public static void InstallPackages()
    {
      if (EditorUtility.DisplayDialog(
                "Install Packages",
                "This will install required packages. Continue?", "Yes", "Cancel"))
      {
        currentIndex = 0;
        InstallNextPackage();
      }
    }
    private static void InstallNextPackage()
    {
      if (currentIndex >= packagesToInstall.Count)
      {
        EditorUtility.ClearProgressBar();
        Debug.Log("All packages installed.");
        return;
      }

      string packageName = packagesToInstall[currentIndex];
      float progress = (float)currentIndex / packagesToInstall.Count;
      EditorUtility.DisplayProgressBar("Installing Packages",
          "Installing: " + packageName, progress);

      request = Client.Add(packageName);
      EditorApplication.update += MonitorRequest;
    }
    private static void MonitorRequest()
    {
      if (request.IsCompleted)
      {
        if (request.Status == StatusCode.Success)
        {
          Debug.Log("Installed: " + request.Result.packageId);
        }
        else if (request.Status >= StatusCode.Failure)
        {
          Debug.LogError("Failed to install " + packagesToInstall[currentIndex] + ": " + request.Error.message);
        }

        EditorApplication.update -= MonitorRequest;
        currentIndex++;
        InstallNextPackage();
      }
    }
  }
}

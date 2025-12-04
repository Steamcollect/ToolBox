using UnityEngine;

public struct MVsToolkitProjectItem
{
    public string Path;
    public string Icon;

    public MVsToolkitProjectItem(string path, string icon = "Folder Icon")
    {
        Path = path;
        Icon = icon;
    }

    public void Draw(Rect rect)
    {

    }
}
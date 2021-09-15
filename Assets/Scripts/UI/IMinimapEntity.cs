using UnityEngine;

public interface IMinimapEntity
{
    public string GetLayerName();
    public MinimapIcon GetMinimapIconInstance();
}

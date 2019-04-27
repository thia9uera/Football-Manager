using UnityEngine;
using UnityEngine.U2D;

public class AtlasManager : MonoBehaviour
{
    public SpriteAtlas Portraits;

    public Sprite GetPortrait(string _name, PlayerAttributes.PlayerPosition _pos = PlayerAttributes.PlayerPosition.Goalkeeper)
    {
        Sprite spr = null;

        spr = Portraits.GetSprite(_name);
        if (spr == null) spr = GetPortrait(_pos);

        return spr;
    }

    public Sprite GetPortrait(PlayerAttributes.PlayerPosition _pos)
    {
        string str = "GK_0";
        switch (_pos)
        {
            case PlayerAttributes.PlayerPosition.Defender: str = "DF_0"; break;
            case PlayerAttributes.PlayerPosition.Midfielder: str = "MC_0"; break;
            case PlayerAttributes.PlayerPosition.Forward: str = "FW_0"; break;
        }
        return Portraits.GetSprite(str);
    }
}

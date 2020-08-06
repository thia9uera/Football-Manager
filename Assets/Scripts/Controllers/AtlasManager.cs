using UnityEngine;
using UnityEngine.U2D;

public class AtlasManager : MonoBehaviour
{
	public static AtlasManager Instance;
	
    public SpriteAtlas Portraits;

	private void Awake()
	{
		if(Instance == null) Instance = this;
	}

    public Sprite GetPortrait(string _name, PlayerPosition _pos = PlayerPosition.Goalkeeper)
    {
        Sprite spr = null;

        spr = Portraits.GetSprite(_name);
        if (spr == null) spr = GetPortrait(_pos);

        return spr;
    }

    public Sprite GetPortrait(PlayerPosition _pos)
    {
        string str = "GK_0";
        switch (_pos)
        {
            case PlayerPosition.Defender: str = "DF_0"; break;
            case PlayerPosition.Midfielder: str = "MC_0"; break;
            case PlayerPosition.Forward: str = "FW_0"; break;
        }
        return Portraits.GetSprite(str);
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "FormationList", menuName = "Data/Formation List", order = 1)]
public class FormationsData : ScriptableObject
{
	public FormationData[] List;
    
	public int GetFormationId(FormationData _data)
	{
		for (int i = 0; i < List.Length; i++)
		{
			if(_data == List[i]) 
			{
				return (int)_data.Type;
				break;
			}
		}
		return 0;
	}
	
	public FormationData GetFormation(FormationType _type)
	{
		for (int i = 0; i < List.Length; i++)
		{
			if(_type == List[i].Type) 
			{
				return List[i];
				break;
			}
		}
		return List[0];
	}
}

public enum FormationType
{
	_3_4_3,
	_4_3_4,
	_2_5_3,
	_4_4_2,
}


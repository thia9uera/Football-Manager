using System.Collections.Generic;

public static class ActionRoll
{
	public static float HeaderPass(PlayerData _player)
	{
		List<float> list = new List<float>();
		
		list.Add(_player.Passing);
		list.Add(_player.Agility);
		list.Add(_player.Vision);
		list.Add(_player.Teamwork);
		list.Add(_player.Heading);
		
		return GetResult(list);
	}
	
	public static float HeaderBlock(PlayerData _player)
	{
		List<float> list = new List<float>();
		
		list.Add(_player.Blocking);
		list.Add(_player.Agility);
		list.Add(_player.Vision);
		list.Add(_player.Heading);
		
		return GetResult(list);
	}
	
	public static float Pass(PlayerData _player)
	{
		List<float> list = new List<float>();
		
		list.Add(_player.Passing);
		list.Add(_player.Agility);
		list.Add(_player.Vision);
		list.Add(_player.Teamwork);
		
		return GetResult(list);
	}
	
	public static float Block(PlayerData _player)
	{
		List<float> list = new List<float>();
		
		list.Add(_player.Blocking);
		list.Add(_player.Agility);
		list.Add(_player.Vision);
		
		return GetResult(list);
	}
	
	public static float LongPass(PlayerData _player)
	{
		List<float> list = new List<float>();
		
		list.Add(_player.Passing);
		list.Add(_player.Agility);
		list.Add(_player.Vision);
		list.Add(_player.Teamwork);
		list.Add(_player.Strength);

		return GetResult(list);
	}
	
	public static float Dribble(PlayerData _player)
	{
		List<float> list = new List<float>();
		
		list.Add(_player.Passing);
		list.Add(_player.Agility);
		list.Add(_player.Vision);
		list.Add(_player.Teamwork);
		list.Add(_player.Strength);

		return GetResult(list);
	}
	
	public static float Tackle(PlayerData _player)
	{
		List<float> list = new List<float>();
		
		list.Add(_player.Tackling);
		list.Add(_player.Agility);
		list.Add(_player.Speed);
		
		return GetResult(list);
	}
	
	public static float Sprint(PlayerData _player)
	{
		List<float> list = new List<float>();
		
		list.Add(_player.Agility);
		list.Add(_player.Speed);

		return GetResult(list);
	}
	
	public static float Cross(PlayerData _player)
	{
		List<float> list = new List<float>();
		
		list.Add(_player.Crossing);
		list.Add(_player.Agility);
		list.Add(_player.Vision);
		list.Add(_player.Teamwork);
		list.Add(_player.Strength);

		return GetResult(list);
	}
	
	public static float Shoot(PlayerData _player)
	{
		List<float> list = new List<float>();
		
		list.Add(_player.Shooting);
		list.Add(_player.Agility);
		list.Add(_player.Strength);

		return GetResult(list);
	}
	
	public static float Header(PlayerData _player)
	{
		List<float> list = new List<float>();
		
		list.Add(_player.Heading);
		list.Add(_player.Agility);
		list.Add(_player.Strength);

		return GetResult(list);
	}
	
	public static float Penalty(PlayerData _player)
	{
		List<float> list = new List<float>();
		
		list.Add(_player.Penalty);
		list.Add(_player.Strength);

		return GetResult(list);
	}
	
	public static float Freekick(PlayerData _player)
	{
		List<float> list = new List<float>();
		
		list.Add(_player.Freekick);
		list.Add(_player.Strength);

		return GetResult(list);
	}
	
	public static float Keeper(PlayerData _player)
	{
		List<float> list = new List<float>();
		
		list.Add(_player.Goalkeeping);
		list.Add(_player.Agility);

		return GetResult(list);
	} 
	
	private static float GetResult(List<float> _list)
	{
		float sum = 0;
		foreach(float value in _list)
		{
			sum += (float)value;
		}
		sum /= _list.Count * 100;
		return sum;
	}
}

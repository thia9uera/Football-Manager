public static class DistanceModifiers
{
	public static float ShotModifier(Zone _zone)
	{
		float distanceModifier = 1f;
		
		switch (_zone)
		{
			default:
				distanceModifier = 0.1f;
				break;
	
			case Zone.Box:
				distanceModifier = 1.25f;
				break;
	
			case Zone.LAM:
			case Zone.RAM:
				distanceModifier = 0.5f;
				break;
	
			case Zone.LCAM:
			case Zone.CAM:
			case Zone.RCAM:
				distanceModifier = 0.65f;
				break;
	
			case Zone.LF:
			case Zone.RF:
				distanceModifier = 0.75f;
				break;
	
			case Zone.ALF:
			case Zone.ARF:
				distanceModifier = 1f;
				break;
	
			case Zone.LCF:
			case Zone.CF:
			case Zone.RCF:
				distanceModifier = 0.9f;
				break;
		}
		
		return distanceModifier;
	}
}

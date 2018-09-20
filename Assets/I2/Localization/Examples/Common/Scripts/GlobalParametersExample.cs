namespace I2.Loc
{

    public class GlobalParametersExample : RegisterGlobalParameters
	{
		public override string GetParameterValue( string ParamName )
        {
            if (ParamName == "WINNER")
                return "BUNDA";            // For your game, get this value from your Game Manager
            
            if (ParamName == "NUM PLAYERS")
                return 5.ToString();

            return null;
        }

	}
}
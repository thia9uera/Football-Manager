namespace I2.Loc
{
    public class GlobalParameters : RegisterGlobalParameters
    {
        public override string GetParameterValue(string ParamName)
        {
            if (ParamName == "PLAYER_1")
                return MainController.Instance.Localization.PLAYER_1;

            if (ParamName == "PLAYER_2")
                return MainController.Instance.Localization.PLAYER_2;


            return null;
        }
    }
}
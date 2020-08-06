namespace I2.Loc
{
    public class GlobalParameters : RegisterGlobalParameters
    {
        public override string GetParameterValue(string ParamName)
        {
            if (ParamName == "PLAYER_1")
                return LocalizationController.Instance.PLAYER_1;

            if (ParamName == "PLAYER_2")
                return LocalizationController.Instance.PLAYER_2;

            if (ParamName == "TEAM_1")
                return LocalizationController.Instance.TEAM_1;

            if (ParamName == "TEAM_2")
                return LocalizationController.Instance.TEAM_2;

            if (ParamName == "ZONE")
                return LocalizationController.Instance.ZONE;

            if (ParamName == "EXTRA_1")
                return LocalizationController.Instance.EXTRA_1;


            return null;
        }
    }
}
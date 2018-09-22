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

            if (ParamName == "TEAM_1")
                return MainController.Instance.Localization.TEAM_1;

            if (ParamName == "TEAM_2")
                return MainController.Instance.Localization.TEAM_2;

            if (ParamName == "ZONE")
                return MainController.Instance.Localization.ZONE;


            if (ParamName == "EXTRA_1")
                return MainController.Instance.Localization.EXTRA_1;


            return null;
        }
    }
}
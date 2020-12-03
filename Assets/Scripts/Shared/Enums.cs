public static class Enums{}

//Names given by Home Team perspective
public enum Zone
{
    OwnGoal = 0,
	BLD, BRD, //DEFENSE CORNERS 1 2
	LD, LCD, CD, RCD, RD, // DEFENSE 3 4 5 6 7
	LDM, LCDM, CDM, RCDM, RDM, // DEFENDING MIDFIELD 8 9 10 11 12
	LM, LCM, CM, RCM, RM, // MIDFIELD 13 14 15 16 17
	LAM, LCAM, CAM, RCAM, RAM, // ATTACKING MIDFIELD 18 19 20 21 22
	LF, LCF, CF, RCF, RF, // ATTACKING 23 24 25 26 27
	ALF, ARF, // ATTACKING CORNERS 28 29
	Box, // 30
}

public enum TeamStrategy
{
    Neutral,
    Offensive,
    Defensive,
    Deadlock,
    WingsOffensive,
    CenterOffensive,
    Crossing,
    Pressure,
    ForceOffside
}
    
public enum AttributeType
{
	Goalkeeping,
	Passing,
	Crossing,
	Tackling,
	Shooting,
	Heading,
	Freekick,
	Penalty,
	Speed,
	Strength,
	Agility,
	Stamina,
	Teamwork,
	Vision,
	Stability,
	Blocking,
	Dribbling,
}

public enum SummaryAttributeType
{
	Attack,
	Defense,
	Physical,
	Tactical
}

public enum PlayerAction
{
	None,
	Dribble,
	Pass,
	LongPass,
	Cross,
	ThroughPass,
	Shot,
	LongShot,
	Tackle,
	Fault,
	Block,
	Save,
	Header,
	Sprint,
}

public enum AltPosition
{
	None,
	Offensive,
	Defensive,
	Left,
	Right,
	LeftDefensive,
	RightDefensive,
	LeftOffensive,
	RightOffensive
}

public enum MatchEvent
{
	None,
	KickOff,
	SecondHalfKickOff,
	Penalty,
	Offside,
	ThrowIn,
	Goal,
	Goalkick,
	CornerKick,
	ShotSaved,
	PenaltySaved,
	ShotMissed,
	GoalAnnounced,
	ScorerAnnounced,
	FreekickTaken,
	ShotOnGoal,
	HalfTime,
	FullTime,
	Fault,
	PenaltyShot,
}

public enum MarkingType
{
	None,
	Distance,
	Close,
	Steal
}

public enum PlayerPosition
{
    Goalkeeper,
    Defender,
    Midfielder,
    Forward,
}

public enum PlayerStrategy
{
    Neutral,
    Defensive,
    Ofensive,
    Left,
    Right,
    LeftDefensive,
    RightDefensive,
    LeftOffensive,
    RightOffensive,
}

public enum SynergyGroup
{
    Neutral,
    Good,
    Evil,
    NeutralGood,
    NeutralEvil
}

public enum NarrationType
{
	Default,
	GoalCelebration
}

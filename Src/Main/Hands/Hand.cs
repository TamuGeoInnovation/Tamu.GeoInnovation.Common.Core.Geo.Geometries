namespace USC.GISResearchLab.Common.Geometries.Hands
{

    public enum HandValues { unknown, left, right }

    public class Hand
    {
        public const int HAND_LEFT = 9;
        public const int HAND_RIGHT = 10;


        public static HandValues GetHandValueFromInt(int handValue)
        {
            HandValues ret = HandValues.unknown;
            switch (handValue)
            {
                case HAND_LEFT:
                    ret = HandValues.left;
                    break;
                case HAND_RIGHT:
                    ret = HandValues.right;
                    break;
                default:
                    ret = HandValues.unknown;
                    break;
            }
            return ret;
        }

        public static int GetHandIntFromHandValue(HandValues handValue)
        {
            int ret = -1;
            switch (handValue)
            {
                case HandValues.left:
                    ret = HAND_LEFT;
                    break;
                case HandValues.right:
                    ret = HAND_RIGHT;
                    break;
                default:
                    ret = -1;
                    break;
            }
            return ret;
        }

        public static string getHandName(HandValues handValue)
        {
            int handInt = GetHandIntFromHandValue(handValue);
            return getHandName(handInt);
        }

        public static string getHandName(int handValue)
        {
            string ret = "";
            switch (handValue)
            {
                case HAND_LEFT:
                    ret = "HAND_LEFT";
                    break;
                case HAND_RIGHT:
                    ret = "HAND_RIGHT";
                    break;
                default:
                    ret = "UNKNOWN HAND: " + handValue;
                    break;
            }
            return ret;
        }
    }
}
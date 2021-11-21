using Assets.Scripts.CharacterControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Assets.Scripts.Utility
{
    public static class Replay
    {
        // properties to serialize
        public static string gameVersion { get; set; }
        public static Dictionary<int, UserInputCollection> inputCollection { get; set; }


        private static string _replayFileDirectory;
        

        public static void InitializeReplay(string version, string replayFileDirectory)
        {
            gameVersion = version;
            _replayFileDirectory = replayFileDirectory;
            inputCollection = new Dictionary<int, UserInputCollection>();
        }
        public static void AddInputForFrame(int frame, UserInputCollection userInput)
        {
            if(IsInputEmpty(userInput) || inputCollection.ContainsKey(frame))
            {
                return;
            }
            inputCollection.Add(frame, userInput);
        }
        public static UserInputCollection GetInputForFrame(int frame)
        {
            return inputCollection.GetValueOrDefault(frame);
        }
        public static void WriteReplayFile()
        {
            string json = SerializeSelf();
            string fileName = string.Format("spiderReplay-{0}.json", DateTime.Now.ToString("yyyy-MM-dd.HH.mm.ss.fff"));
            string fileLocation = Path.Combine(_replayFileDirectory, fileName);
            using (StreamWriter sw = new StreamWriter(fileLocation, true))
            {
                sw.WriteLine(json);
            }
        }
        public static void DeSerializeFromReplayFile(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                int currentKey = -1;
                UserInputCollection currentCollection = new UserInputCollection();

                while ((line = reader.ReadLine()) != null)
                {
                    var match = GetKvpFromJsonLine(line);
                    if(match.success)
                    {
                        if(match.key == "gameVersion")
                        {
                            gameVersion = match.value;
                        }
                        if(match.key == "inputCollection")
                        {
                            // create a new input collection
                            inputCollection = new Dictionary<int, UserInputCollection>();
                        }
                        if (match.key == "Key")
                        {
                            // a new frame
                            // add the old one to the stack and create a new one
                            if (currentKey != -1)
                            {
                                inputCollection.Add(currentKey, currentCollection);
                            }
                            currentKey = int.Parse(match.value);
                            currentCollection = new UserInputCollection();
                            
                        }
                        if (match.key == "moveHPressure")
                        {
                            currentCollection.moveHPressure = int.Parse(match.value);
                        }
                        if (match.key == "moveVPressure")
                        {
                            currentCollection.moveVPressure = int.Parse(match.value);
                        }
                        if (match.key == "isJumpPressed")
                        {
                            currentCollection.isJumpPressed = bool.Parse(match.value);
                        }
                        if (match.key == "isJumpReleased")
                        {
                            currentCollection.isJumpReleased = bool.Parse(match.value);
                        }
                        if (match.key == "isJumpHeldDown")
                        {
                            currentCollection.isJumpHeldDown = bool.Parse(match.value);
                        }
                        if (match.key == "isGrappleButtonPressed")
                        {
                            currentCollection.isGrappleButtonPressed = bool.Parse(match.value);
                        }
                        if (match.key == "isGrappleButtonReleased")
                        {
                            currentCollection.isGrappleButtonReleased = bool.Parse(match.value);
                        }
                        if (match.key == "isGrappleButtonHeldDown")
                        {
                            currentCollection.isGrappleButtonHeldDown = bool.Parse(match.value);
                        }
                    }
                }
                // add the last entry onto the stack
                inputCollection.Add(currentKey, currentCollection);
            }
        }
        private static (bool success, string key,string value) GetKvpFromJsonLine(string line)
        {
            //string expression = "\b([\\w\\s\\d_\"]{ 1,}):([\\w\\s\\d_]{ 1,})";
            string expression = @"[""]{0,}([\w\\d\s]{1,})[""]{0,}[\s]{0,}:[\s]{0,}[""]{0,}([\d\w\.-]{1,})[""]{0,}";
            MatchCollection mc = Regex.Matches(line, expression);
            if(mc != null && mc.Count > 0)
            {
                string key = mc[0].Groups[1].ToString();
                string value = mc[0].Groups[2].ToString();
                return (true, key, value);
            }
            return (false, null, null);
        }
        private static string SerializeSelf()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine(string.Format("\"gameVersion\": \"{0}\",\n", gameVersion));
            sb.AppendLine("\"inputCollection\": {\n");
            foreach(KeyValuePair<int,UserInputCollection> row in inputCollection)
            {
                sb.AppendLine(string.Format("\"Key\": {0},", row.Key.ToString()));
                sb.AppendLine("\"UserInputCollection\": {");
                sb.AppendLine(string.Format("\"moveHPressure\": {0},", row.Value.moveHPressure.ToString()));
                sb.AppendLine(string.Format("\"moveVPressure\": {0},", row.Value.moveVPressure.ToString()));
                sb.AppendLine(string.Format("\"isJumpPressed\": {0},", row.Value.isJumpPressed ? "true" : "false"));
                sb.AppendLine(string.Format("\"isJumpReleased\": {0},", row.Value.isJumpReleased ? "true" : "false"));
                sb.AppendLine(string.Format("\"isJumpHeldDown\": {0},", row.Value.isJumpHeldDown ? "true" : "false"));
                sb.AppendLine(string.Format("\"isGrappleButtonPressed\": {0},", row.Value.isGrappleButtonPressed ? "true" : "false"));
                sb.AppendLine(string.Format("\"isGrappleButtonReleased\": {0},", row.Value.isGrappleButtonReleased ? "true" : "false"));
                sb.AppendLine(string.Format("\"isGrappleButtonHeldDown\": {0},", row.Value.isGrappleButtonHeldDown ? "true" : "false"));
                sb.AppendLine("}"); // end UserInputCollection for that row
            }
            sb.AppendLine("}"); // end dictionary
            sb.AppendLine("}"); // end replay
            return sb.ToString();
        }
        private static bool IsInputEmpty(UserInputCollection input)
        {
            if (input.moveHPressure > 0.01) return false;
            if (input.moveHPressure < -0.01) return false;
            if (input.moveVPressure > 0.01) return false;
            if (input.moveVPressure < -0.01) return false;
            if (input.isJumpPressed) return false;
            if (input.isJumpReleased) return false;
            if (input.isJumpHeldDown) return false;
            if (input.isGrappleButtonPressed) return false;
            if (input.isGrappleButtonReleased) return false;
            if (input.isGrappleButtonHeldDown) return false;
            return true;
        }
    }
}

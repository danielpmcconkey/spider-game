using Assets.Scripts.CharacterControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public struct ReplayFrame
    {
        public UserInputCollection inputCollection;
        public Vector2 position;
        public Vector2 velocity;

    }
    public static class Replay
    {
        // properties to serialize
        public static string gameVersion { get; set; }
        public static Dictionary<int, ReplayFrame> replayFrames { get; set; }
        

        private static string _replayFileDirectory;
        
        public static void InitializeReplay(string version, string replayFileDirectory)
        {
            gameVersion = version;
            _replayFileDirectory = replayFileDirectory;
            replayFrames = new Dictionary<int, ReplayFrame>();
        }
        public static void AddInputForFrame(int frame, ReplayFrame replayFrame)
        {
            if(replayFrames.ContainsKey(frame))
            {
                return;
            }
            replayFrames.Add(frame, replayFrame);
        }
        public static ReplayFrame GetInputForFrame(int frame)
        {
            return replayFrames.GetValueOrDefault(frame);
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
                ReplayFrame replayFrame = new ReplayFrame();

                while ((line = reader.ReadLine()) != null)
                {
                    var match = GetKvpFromJsonLine(line);
                    if(match.success)
                    {
                        if(match.key == "gameVersion")
                        {
                            gameVersion = match.value;
                        }
                        if (match.key == "replayFrames")
                        {
                            // create a new input collection
                            replayFrames = new Dictionary<int, ReplayFrame>();
                        }
                        if (match.key == "Key")
                        {
                            // a new frame
                            // add the old one to the stack and create a new one
                            if (currentKey != -1)
                            {
                                replayFrames.Add(currentKey, replayFrame);
                            }
                            currentKey = int.Parse(match.value);
                            replayFrame = new ReplayFrame();
                            
                        }
                        if (match.key == "moveHPressure")
                        {
                            replayFrame.inputCollection.moveHPressure = float.Parse(match.value);
                        }
                        if (match.key == "mouseX")
                        {
                            replayFrame.inputCollection.mouseX = float.Parse(match.value);
                        }
                        if (match.key == "mouseY")
                        {
                            replayFrame.inputCollection.mouseY = float.Parse(match.value);
                        }
                        if (match.key == "moveVPressure")
                        {
                            replayFrame.inputCollection.moveVPressure = float.Parse(match.value);
                        }
                        if (match.key == "isJumpPressed")
                        {
                            replayFrame.inputCollection.isJumpPressed = bool.Parse(match.value);
                        }
                        if (match.key == "isJumpReleased")
                        {
                            replayFrame.inputCollection.isJumpReleased = bool.Parse(match.value);
                        }
                        if (match.key == "isJumpHeldDown")
                        {
                            replayFrame.inputCollection.isJumpHeldDown = bool.Parse(match.value);
                        }
                        if (match.key == "isGrappleButtonPressed")
                        {
                            replayFrame.inputCollection.isGrappleButtonPressed = bool.Parse(match.value);
                        }
                        if (match.key == "isGrappleButtonReleased")
                        {
                            replayFrame.inputCollection.isGrappleButtonReleased = bool.Parse(match.value);
                        }
                        if (match.key == "isGrappleButtonHeldDown")
                        {
                            replayFrame.inputCollection.isGrappleButtonHeldDown = bool.Parse(match.value);
                        }
                        if (match.key == "positionX")
                        {
                            if (replayFrame.position == null) replayFrame.position = Vector2.zero;
                            replayFrame.position.x = float.Parse(match.value);
                        }
                        if (match.key == "positionY")
                        {
                            if (replayFrame.position == null) replayFrame.position = Vector2.zero;
                            replayFrame.position.y = float.Parse(match.value);
                        }
                        if (match.key == "velocityX")
                        {
                            if (replayFrame.velocity == null) replayFrame.velocity = Vector2.zero;
                            replayFrame.velocity.x = float.Parse(match.value);
                        }
                        if (match.key == "velocityY")
                        {
                            if (replayFrame.velocity == null) replayFrame.velocity = Vector2.zero;
                            replayFrame.velocity.y = float.Parse(match.value);
                        }
                    }
                }
                // add the last entry onto the stack
                replayFrames.Add(currentKey, replayFrame);
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
            sb.AppendLine("\"replayFrames\": {\n");
            foreach(KeyValuePair<int,ReplayFrame> row in replayFrames)
            {
                sb.AppendLine(string.Format("\"Key\": {0},", row.Key.ToString()));
                sb.AppendLine("\"ReplayFrame\": {");
                sb.AppendLine(string.Format("\"moveHPressure\": {0},", row.Value.inputCollection.moveHPressure.ToString()));
                sb.AppendLine(string.Format("\"moveVPressure\": {0},", row.Value.inputCollection.moveVPressure.ToString()));
                sb.AppendLine(string.Format("\"isJumpPressed\": {0},", row.Value.inputCollection.isJumpPressed ? "true" : "false"));
                sb.AppendLine(string.Format("\"isJumpReleased\": {0},", row.Value.inputCollection.isJumpReleased ? "true" : "false"));
                sb.AppendLine(string.Format("\"isJumpHeldDown\": {0},", row.Value.inputCollection.isJumpHeldDown ? "true" : "false"));
                sb.AppendLine(string.Format("\"isGrappleButtonPressed\": {0},", row.Value.inputCollection.isGrappleButtonPressed ? "true" : "false"));
                sb.AppendLine(string.Format("\"isGrappleButtonReleased\": {0},", row.Value.inputCollection.isGrappleButtonReleased ? "true" : "false"));
                sb.AppendLine(string.Format("\"isGrappleButtonHeldDown\": {0},", row.Value.inputCollection.isGrappleButtonHeldDown ? "true" : "false"));
                sb.AppendLine(string.Format("\"mouseX\": {0},", row.Value.inputCollection.mouseX.ToString()));
                sb.AppendLine(string.Format("\"mouseY\": {0},", row.Value.inputCollection.mouseY.ToString()));
                sb.AppendLine(string.Format("\"positionX\": {0},", row.Value.position.x.ToString()));
                sb.AppendLine(string.Format("\"positionY\": {0},", row.Value.position.y.ToString()));
                sb.AppendLine(string.Format("\"velocityX\": {0},", row.Value.velocity.x.ToString()));
                sb.AppendLine(string.Format("\"velocityY\": {0},", row.Value.velocity.y.ToString()));
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
            if (input.mouseX != 0) return false;
            if (input.mouseY != 0) return false;
            return true;
        }
    }
}

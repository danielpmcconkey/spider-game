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
    [Serializable]
    public struct ReplayFrame
    {
        public UserInputCollection inputCollection;
        public Vector2 position;
        public Vector2 velocity;
    }
    [Serializable]
    public class ReplayFramesCollection
    {
        public string gameVersion;
        public ReplayFrame[] arrayOfFrames;
        public Dictionary<int, ReplayFrame> replayFrames;
    }

    public static class Replay
    {
        private static ReplayFramesCollection _replayFrames;
        private static string _replayFileDirectory;
        
        public static void InitializeReplay(string version, string replayFileDirectory)
        {
            _replayFrames = new ReplayFramesCollection();
            _replayFrames.replayFrames = new Dictionary<int, ReplayFrame>();
            _replayFrames.gameVersion = version;
            _replayFileDirectory = replayFileDirectory;
        }
        public static void AddInputForFrame(int frame, ReplayFrame replayFrame)
        {
            if(_replayFrames.replayFrames.ContainsKey(frame))
            {
                return;
            }
            _replayFrames.replayFrames.Add(frame, replayFrame);
        }
        public static ReplayFrame GetInputForFrame(int frame)
        {
            return _replayFrames.replayFrames.GetValueOrDefault(frame);
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
                
                StringBuilder json = new StringBuilder();
                while ((line = reader.ReadLine()) != null)
                {
                    json.AppendLine(line);
                }
                _replayFrames = JsonUtility.FromJson<ReplayFramesCollection>(json.ToString());
                _replayFrames.replayFrames = new Dictionary<int, ReplayFrame>();
                for (int i = 0; i < _replayFrames.arrayOfFrames.Length; i++)
                {
                    _replayFrames.replayFrames.Add(i, _replayFrames.arrayOfFrames[i]);
                }
            }
        }
        private static string SerializeSelf()
        {
            ReplayFramesCollection serializableFrames = new ReplayFramesCollection();
            serializableFrames.gameVersion = _replayFrames.gameVersion;
            int lastFrame = _replayFrames.replayFrames.Max(x => x.Key);
            serializableFrames.arrayOfFrames = new ReplayFrame[lastFrame + 1];
            foreach (var frame in _replayFrames.replayFrames) serializableFrames.arrayOfFrames[frame.Key] = frame.Value;
            return JsonUtility.ToJson(serializableFrames);
        }
    }
}

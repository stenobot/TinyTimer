using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;
using System.Collections.ObjectModel;

namespace TinyTimer.DataModel
{
    public class SoundPlayer
    {
        const string baseUri = "ms-appx:///Assets/Sounds/";
        const string audioFileExt = ".wav";
        const string doneBaseFileName = "done";
        const string countdownBaseFileName = "countdown";

        const int countdownSoundsNum = 7;
        const int doneSoundsNum = 10;

        // main audio graph and output node
        private AudioGraph graph;
        private AudioDeviceOutputNode outputNode;
        private ObservableCollection<AudioFileInputNode> InputNodes;
        private Dictionary<string, AudioFileInputNode> FileInputNodesDictionary;

        private Random random;


        public void PlaySound(int index)
        {
            InputNodes[index].Reset();
            InputNodes[index].Start();
        }

        public void PlayRandomSound()
        {
            if (random == null)
                random = new Random();

            int randomIndex = random.Next(((countdownSoundsNum * 2)), (((countdownSoundsNum * 2) + doneSoundsNum) - 1));

            InputNodes[randomIndex].Reset();
            InputNodes[randomIndex].Start();
        }

        public void StopAllSounds()
        {
            outputNode.Stop();
        }


        public async Task InitializeSounds()
        {
            InputNodes = new ObservableCollection<AudioFileInputNode>();
            FileInputNodesDictionary = new Dictionary<string, AudioFileInputNode>();

            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media);
            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);

            if (result.Status == AudioGraphCreationStatus.Success)
            {
                graph = result.Graph;
                CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await graph.CreateDeviceOutputNodeAsync();


                // make sure the audio output is available
                if (deviceOutputNodeResult.Status == AudioDeviceNodeCreationStatus.Success)
                {
                    outputNode = deviceOutputNodeResult.DeviceOutputNode;
                    graph.ResetAllNodes();

                    for (int i = 0; i < countdownSoundsNum; i++)
                    {
                        await CreateInputNodeFromFile(baseUri + countdownBaseFileName + "0" + (i + 1).ToString() + "-" + "1" + audioFileExt);
                        await CreateInputNodeFromFile(baseUri + countdownBaseFileName + "0" + (i + 1).ToString() + "-" + "2" + audioFileExt);
                    }

                    for (int j = 0; j < doneSoundsNum; j++)
                    {
                        await CreateInputNodeFromFile(baseUri + doneBaseFileName + ((j >= 9) ? "" : "0") + (j + 1).ToString() + audioFileExt);
                    }

                    graph.Start();
                }
            }
        }


        private async Task CreateInputNodeFromFile(string uri)
        {
            StorageFile soundFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));

            CreateAudioFileInputNodeResult fileInputNodeResult = await graph.CreateFileInputNodeAsync(soundFile);

            if (AudioFileNodeCreationStatus.Success == fileInputNodeResult.Status)
            {
               // FileInputNodesDictionary.Add(soundFile.Name, fileInputNodeResult.FileInputNode);
                fileInputNodeResult.FileInputNode.Stop();
                fileInputNodeResult.FileInputNode.AddOutgoingConnection(outputNode);
                fileInputNodeResult.FileInputNode.LoopCount = 0;
                //FileInputNodesDictionary.Add(soundFile.Name, fileInputNodeResult.FileInputNode);
                InputNodes.Add(fileInputNodeResult.FileInputNode);
            }
        }
    }
}

using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class OutputAudioRecorder5 : MonoBehaviour
{
    public GameObject vibrator;
    private MicInput inputActions;
    public bool visible = false;
    internal string FILENAME;
    private int outputRate;
    private int headerSize = 44; //default for uncompressed wav
    private String fileName;
    private bool recOutput = false;
    private bool isStart = true;
    private bool countdownBool = false;
    private FileStream fileStream;
    public float countdownTime = 10f;
    private float currentTime;
    float[] tempDataSource;
    public TMP_Text timerText;
    private void Awake()
    {
        inputActions = new MicInput();
        vibrator.SetActive(false);
        Debug.Log("awake");
        outputRate = AudioSettings.outputSampleRate;

        // Vérification et récupération automatique de TMP_Text si non assigné
        if (timerText == null)
        {
            timerText = GameObject.Find("TimerText")?.GetComponent<TMP_Text>();
            if (timerText == null)
            {
                Debug.LogError("TMP_Text component is not assigned and couldn't be found automatically.");
            }
            else
            {
                Debug.Log("TMP_Text component found and assigned.");
            }
        }
    }
    void Update()
    {
        if (currentTime > 0 && countdownBool)
        {
            currentTime -= Time.deltaTime; // Décrémente le temps actuel
            timerText.text = FormatTime(currentTime); // Met à jour le texte de l'UI
            // Debug.Log(currentTime);
            if (currentTime <= 0)
            {
                currentTime = 0;
                WriteHeader();
                Debug.Log("TIME'S UP!");
            }
        }
    }

    public void StartRecording()
    {
        if (isStart)
        {
            currentTime = countdownTime;
            FILENAME = "record " + UnityEngine.Random.Range(1, 1000);
            fileName = Path.GetFileNameWithoutExtension(FILENAME) + ".mp3";
            if (!recOutput)
            {
                StartWriting(fileName);
                recOutput = true;
                Debug.Log("Start Recording");
            }
            else
            {
                Debug.Log("Recording is in progress already");
            }
            isStart = false;
            countdownBool = true;
        }
        else
        {
            ResumeRecording();
            countdownBool = true;
        }
    }

    public void PauseRecording()
    {
        recOutput = false;
        countdownBool = false;
        Debug.Log("Pause Recording");
    }

    public void ResumeRecording()
    {
        recOutput = true;
        Debug.Log("Resume Recording");
    }

    private void StartWriting(String name)
    {
        fileStream = new FileStream(Application.persistentDataPath + "/" + name, FileMode.Create);
        var emptyByte = new byte();
        for (int i = 0; i < headerSize; i++) //preparing the header
        {
            fileStream.WriteByte(emptyByte);
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (recOutput)
        {
            ConvertAndWrite(data); //audio data is interlaced
        }
    }

    private void ConvertAndWrite(float[] dataSource)
    {
        var intData = new Int16[dataSource.Length];
        //converting in 2 steps : float[] to Int16[], //then Int16[] to Byte[]
        var bytesData = new Byte[dataSource.Length * 2];
        //bytesData array is twice the size of
        //dataSource array because a float converted in Int16 is 2 bytes.
        var rescaleFactor = 32767; //to convert float to Int16
        for (var i = 0; i < dataSource.Length; i++)
        {
            intData[i] = (Int16)(dataSource[i] * rescaleFactor);
            var byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }
        fileStream.Write(bytesData, 0, bytesData.Length);
        tempDataSource = new float[dataSource.Length];
        tempDataSource = dataSource;
    }

    private void WriteHeader()
    {
        fileStream.Seek(0, SeekOrigin.Begin);
        var riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);
        var chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);
        var wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);
        var fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);
        var subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);
        UInt16 two = 2;
        UInt16 one = 1;
        var audioFormat = BitConverter.GetBytes(one);
        fileStream.Write(audioFormat, 0, 2);
        var numChannels = BitConverter.GetBytes(two);
        fileStream.Write(numChannels, 0, 2);
        var sampleRate = BitConverter.GetBytes(outputRate);
        fileStream.Write(sampleRate, 0, 4);
        var byteRate = BitConverter.GetBytes(outputRate * 4);
        fileStream.Write(byteRate, 0, 4);
        UInt16 four = 4;
        var blockAlign = BitConverter.GetBytes(four);
        fileStream.Write(blockAlign, 0, 2);
        UInt16 sixteen = 16;
        var bitsPerSample = BitConverter.GetBytes(sixteen);
        fileStream.Write(bitsPerSample, 0, 2);
        var dataString = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(dataString, 0, 4);
        var subChunk2 = BitConverter.GetBytes(fileStream.Length - headerSize);
        fileStream.Write(subChunk2, 0, 4);
        Debug.Log("Writing file");
        fileStream.Close();
    }

    private string FormatTime(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
    }

    private void OnEnable()
    {
        inputActions.Mic.ShowHide.performed += OnInteract;
        inputActions.Mic.Enable();
        // Debug.Log("enable");
    }



    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (visible == false)
            {
                vibrator.SetActive(true);
                StartRecording();
                // Debug.Log("Toggled vibrator. New state: visible");
                visible = true;
            }
            else
            {
                vibrator.SetActive(false);
                PauseRecording();
                // Debug.Log("Toggled vibrator. New state: invisble");
                visible = false;
            }
        }
    }

}
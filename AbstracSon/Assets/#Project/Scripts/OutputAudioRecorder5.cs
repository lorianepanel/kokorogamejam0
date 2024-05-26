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
    private const int headerSize = 44; //default for uncompressed wav
    private string fileName;
    private bool recOutput = false;
    private bool isStart = true;
    private bool countdownBool = false;
    private FileStream fileStream;
    public float countdownTime = 10f;
    private float currentTime;
    float[] tempDataSource;
    public TMP_Text timerText;
    public TMP_InputField fileNameInputField; // Add this line

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

        // Vérification et récupération automatique de TMP_InputField si non assigné
        if (fileNameInputField == null)
        {
            fileNameInputField = GameObject.Find("FileNameInputField")?.GetComponent<TMP_InputField>();
            if (fileNameInputField == null)
            {
                Debug.LogError("TMP_InputField component is not assigned and couldn't be found automatically.");
            }
            else
            {
                Debug.Log("TMP_InputField component found and assigned.");
            }
        }
    }

    void Update()
    {
        if (currentTime > 0 && countdownBool)
        {
            currentTime -= Time.deltaTime; // Décrémente le temps actuel
            timerText.text = FormatTime(currentTime); // Met à jour le texte de l'UI
            if (currentTime <= 0)
            {
                currentTime = 0;
                WriteHeader();
                SaveRecordingToFile();
                Debug.Log("TIME'S UP!");
            }
        }
    }

    public void StartRecording()
    {
        if (isStart)
        {
            currentTime = countdownTime;
            FILENAME = string.IsNullOrWhiteSpace(fileNameInputField.text) ? "One_Tape_" + UnityEngine.Random.Range(1, 1000) : fileNameInputField.text;
            fileName = Path.GetFileNameWithoutExtension(FILENAME) + ".wav"; // Change .mp3 to .wav for uncompressed audio
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

    private void StartWriting(string name)
    {
        fileStream = new FileStream(Application.persistentDataPath + "/" + name, FileMode.Create);
        byte emptyByte = new byte();
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
        Int16[] intData = new Int16[dataSource.Length];
        byte[] bytesData = new byte[dataSource.Length * 2];
        int rescaleFactor = 32767; //to convert float to Int16

        for (int i = 0; i < dataSource.Length; i++)
        {
            intData[i] = (Int16)(dataSource[i] * rescaleFactor);
            byte[] byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }
        fileStream.Write(bytesData, 0, bytesData.Length);
        tempDataSource = new float[dataSource.Length];
        tempDataSource = dataSource;
    }

    private void WriteHeader()
    {
        fileStream.Seek(0, SeekOrigin.Begin);
        fileStream.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"), 0, 4);
        fileStream.Write(BitConverter.GetBytes(fileStream.Length - 8), 0, 4);
        fileStream.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"), 0, 4);
        fileStream.Write(System.Text.Encoding.UTF8.GetBytes("fmt "), 0, 4);
        fileStream.Write(BitConverter.GetBytes(16), 0, 4);
        fileStream.Write(BitConverter.GetBytes((ushort)1), 0, 2);
        fileStream.Write(BitConverter.GetBytes((ushort)2), 0, 2);
        fileStream.Write(BitConverter.GetBytes(outputRate), 0, 4);
        fileStream.Write(BitConverter.GetBytes(outputRate * 4), 0, 4);
        fileStream.Write(BitConverter.GetBytes((ushort)4), 0, 2);
        fileStream.Write(BitConverter.GetBytes((ushort)16), 0, 2);
        fileStream.Write(System.Text.Encoding.UTF8.GetBytes("data"), 0, 4);
        fileStream.Write(BitConverter.GetBytes(fileStream.Length - headerSize), 0, 4);
        Debug.Log("Writing file");
        fileStream.Close();
    }

    private void SaveRecordingToFile()
    {
        string filePath = Application.persistentDataPath + "/" + fileName;
        byte[] fileData = File.ReadAllBytes(filePath);
        SaveFile(fileName, fileData);
    }

    public void SaveFile(string fileName, byte[] fileData)
    {
        string downloadsPath = GetDownloadsPath();
        if (string.IsNullOrEmpty(downloadsPath))
        {
            Debug.LogError("Could not find the Downloads folder.");
            return;
        }

        string fullPath = Path.Combine(downloadsPath, fileName);

        try
        {
            File.WriteAllBytes(fullPath, fileData);
            Debug.Log("File saved successfully: " + fullPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save file: " + e.Message);
        }
    }

    private string GetDownloadsPath()
    {
        string path = string.Empty;

        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        }
        else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Downloads");
        }
        else if (Application.platform == RuntimePlatform.LinuxPlayer)
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Downloads");
        }

        return path;
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
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            visible = !visible;
            vibrator.SetActive(visible);
            if (visible)
            {
                StartRecording();
            }
            else
            {
                PauseRecording();
            }
        }
    }
}

using System.Collections;
using UnityEngine;

public class NumberCheckerScript : MonoBehaviour
{
	public KMAudio Audio;
	public KMSelectable[] Buttons;
	public GameObject[] CorrectLEDs;
	public GameObject[] OffLEDs;
	public TextMesh thing;

	private int Iteration;
	private string NotNumbers = "!@#$%^&*()_-+=`~QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm,<.>?/\"':;[]{}|\\ ";
	private bool Validity;

	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;

	private void Awake()
	{
		moduleId = moduleIdCounter++;
		for (int i = 0; i < Buttons.Length; i++)
		{
			KMSelectable btn = Buttons[i];
			Buttons[i].OnInteract += delegate()
			{
				ButtonPress(btn);
				return false;
			};
		}
	}

	private void Start()
	{
		Generation();
	}

	private void Generation()
	{
		if (Random.Range(0, 2) == 1)
		{
			Validity = true;
			thing.text = Random.Range(-4948, 65536).ToString();
		}
		else
		{
			Validity = false;
			thing.text = string.Empty;
			for (int i = 0; i < Random.Range(1, 7); i++)
				thing.text += NotNumbers[Random.Range(0, NotNumbers.Length)].ToString();
		}
		Debug.LogFormat("[Number Checker #{0}] It says {1}", moduleId, thing.text);
		Debug.LogFormat("[Number Checker #{0}] You should press {1}", moduleId, Validity ? "Number" : "Not Number");
	}

	private void ButtonPress(KMSelectable Button)
	{
		Audio.PlayGameSoundAtTransform(0, Button.transform);
		Button.AddInteractionPunch(50f);
		if (moduleSolved)
			return;
		Debug.LogFormat("[Number Checker #{0}] You pressed {1}", moduleId, Button == Buttons[0] ? "Number" : "Not Number");
		if (Button == Buttons[0] && Validity)
		{
			Iteration++;
			if (Iteration == 3)
				Solve();
			else
			{
				CorrectLEDs[Iteration - 1].SetActive(true);
				OffLEDs[Iteration - 1].SetActive(false);
				Generation();
			}
			return;
		}
		if (Button == Buttons[1] && !Validity)
		{
			Iteration++;
			if (Iteration == 3)
				Solve();
			else
			{
				CorrectLEDs[Iteration - 1].SetActive(true);
				OffLEDs[Iteration - 1].SetActive(false);
				Generation();
			}
			return;
		}
		Audio.PlaySoundAtTransform("Wrong", transform);
	}

	private void Solve()
	{
		GetComponent<KMBombModule>().HandlePass();
		moduleSolved = true;
		Debug.LogFormat("[Number Checker #{0}] Module solved", moduleId);
		CorrectLEDs[2].SetActive(true);
		OffLEDs[2].SetActive(false);
	}

	//twitch plays
	#pragma warning disable 414
	private readonly string TwitchHelpMessage = "Use !{0} (Number)/(Not Number) to press either the left or the right button.";
	#pragma warning restore 414

	private IEnumerator ProcessTwitchCommand(string Command)
	{
		Command = Command.Trim().ToUpper();
		yield return null;
		if (Command == "IT SHAYS NUMBAH FAQER!" || Command == "IT SHAYS NUMBAH!" || Command == "NUMBER")
		{
			Buttons[0].OnInteract();
			yield return new WaitForSecondsRealtime(0.1f);
		}
		else if (Command == "IT DOESNT SHAYS NUMBAH FAQER!" || Command == "IT SHAYS NOT NUMBAH!" || Command == "NOT NUMBER")
		{
			Buttons[1].OnInteract();
			yield return new WaitForSecondsRealtime(0.1f);
		}
		else
			yield return "sendtochaterror I don't understand!";
	}

	private IEnumerator TwitchHandleForcedSolve()
	{
		while (!moduleSolved)
		{
			if (Validity)
				Buttons[0].OnInteract();
			else
				Buttons[1].OnInteract();
			yield return new WaitForSecondsRealtime(0.1f);
		}
	}
}
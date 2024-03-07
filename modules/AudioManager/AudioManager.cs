using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime;
using System.Security;
using System.Threading.Tasks;
using System.Xml.Linq;

public partial class AudioManager : Node
{
	public static AudioManager Instance { get; private set; }

	[Export] public Sound[] sounds;

	private List<Sound> musicList = new List<Sound>();

	private Sound currentPlayingTrack;
	private Sound trackToPlay;


	public override void _EnterTree()
	{

		if(Instance != null)
		{
			//
			//("More than one ", Instance.Name);

		}
		else
		{
			Instance = this;
		}

		SetupSoundsAndMusic();

	}


    public void PlayMusic(string name)
	{
		//Find the track you want to play
		for(int i = 0;  i < musicList.Count; i++)
		{
			if (musicList[i].name == name)
			{
				trackToPlay = musicList[i];
			}
		}

		//Find the track that's currently playing
		for(int i = 0; i < musicList.Count; i++)
		{
			if (musicList[i].ghostIsPlaying)
			{
				currentPlayingTrack = musicList[i];
			}
		}

		Fade(currentPlayingTrack, trackToPlay);

		
	}

	private void Fade(Sound trackToFadeOut, Sound trackToFadeIn)
	{

		trackToFadeIn.player.VolumeDb = -60;
		trackToFadeIn.player.Play();
		trackToFadeIn.ghostIsPlaying = true;

		Tween fader = GetTree().CreateTween();
		fader.SetParallel(true);
		if(currentPlayingTrack != null)
		{
			currentPlayingTrack.ghostIsPlaying = false;
			fader.TweenProperty(trackToFadeOut.player, "volume_db", -60, 1.0f).SetEase(Tween.EaseType.Out);
		}
		fader.TweenProperty(trackToFadeIn.player, "volume_db", 0, 1.0f).SetEase(Tween.EaseType.In);


	}

	public void Play(string name)
	{
		foreach (Sound s in sounds)
		{
			if (s.name == name)
				s.player.Play();

            
		}
	}

	public void Stop(string name)
	{
		foreach (Sound s in sounds)
		{
			if (s.name == name)
			{
				s.player.Stop();
			}
		}
	}

	void SetupSoundsAndMusic()
	{
		foreach (Sound s in sounds)
		{
			AudioStreamPlayer player = new AudioStreamPlayer();
			AddChild(player);
			player.Stream = s.AudioStream;
			player.VolumeDb = s.volume;
			player.PitchScale = s.pitch;
			s.player = player;
			player.Bus = s.bus.ToString();

			if (s.bus == Buses.Music)
			{
				musicList.Add(s);

			}

		}
	}




}


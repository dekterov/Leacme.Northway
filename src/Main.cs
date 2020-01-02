// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using Godot;
using System;

public class Main : Spatial {

	public AudioStreamPlayer Audio { get; } = new AudioStreamPlayer();

	private void InitSound() {
		if (!Lib.Node.SoundEnabled) {
			AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), true);
		}
	}

	public override void _Notification(int what) {
		if (what is MainLoop.NotificationWmGoBackRequest) {
			GetTree().ChangeScene("res://scenes/Menu.tscn");
		}
	}

	public override void _Ready() {
		GetNode<WorldEnvironment>("sky").Environment.BackgroundColor = new Color(Lib.Node.BackgroundColorHtmlCode);
		InitSound();
		AddChild(Audio);

		if (Input.GetGravity().Length() < 0.1) {
			GetNode<Spatial>("compass_body").RotateZ(Mathf.Deg2Rad(90));
		}

	}

	public override void _Process(float delta) {

		if (Input.GetGravity().Length() > 0.1) {
			var tg = -Input.GetGravity().Normalized();
			tg.y = 0;
			GetNode<Spatial>("compass_body").Rotation = tg;
		}

		if (Input.GetMagnetometer().Length() > 0.1) {
			Vector3 tRot = new Vector3();
			var bodyRot = GetNode<Spatial>("compass_body").GetNode<MeshInstance>("Main").Rotation;
			tRot.x = bodyRot.x;
			tRot.z = bodyRot.z;

			var mag = Input.GetMagnetometer().Normalized();

			Vector3 grav;
			if (Input.GetGravity().Length() > 0.1) {
				grav = Input.GetGravity().Normalized();
			} else {
				grav = new Vector3(0, -9.81f, 0).Normalized();
			}
			var rawRot = grav.Cross(mag).Normalized();

			tRot.y = (float)Math.Atan2(rawRot.y, rawRot.x);

			GetNode<Spatial>("compass_body").GetNode<MeshInstance>("Arrow").Rotation = tRot;

		}

	}

}

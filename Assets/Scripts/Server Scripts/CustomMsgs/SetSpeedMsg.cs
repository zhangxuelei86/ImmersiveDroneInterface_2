﻿using ROSBridgeLib.std_msgs;
using ROSBridgeLib.interface_msgs;
using System.Collections;
using SimpleJSON;
using UnityEngine;
using System;

namespace ROSBridgeLib
{


	public class SetSpeedMsg : ROSBridgeMsg
	{

		private int _drone_id;
		private bool _success;
		private string _meta_data;

		public SetSpeedMsg(JSONNode msg)
		{
			msg = msg["values"];
			_drone_id = msg["id"].AsInt;
			_success = msg["success"].AsBool;
			_meta_data = msg["message"].ToString();
		}

		public static string getMessageType()
		{
			return "/isaacs_server/set_speed";

		}

		public bool getSuccess()
		{
			return _success;
		}


		public int getDroneId()
		{
			return _drone_id;
		}

		public string getMetaData()
		{
			return _meta_data;
		}


	}
}
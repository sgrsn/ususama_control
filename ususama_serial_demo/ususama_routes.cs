using System;
using RoboticsCommon;

namespace ususama_routes
{
  public enum CleanState
  { 
		Move,
		Stop,
		Clean,
	};
  public struct Pose2D
  {
		public float x;
		public float y;
		public float theta;
		public CleanState state;
	};
	public class UsusamaRoutes
	{
		public UsusamaRoutes()
		{
			ref_poses.Add(new Pose2D((250f + 1200f - 500f) / 1000, 0f, 0f, Move));
			ref_poses.Add(new Pose2D((250f + 1200f - 500f) / 1000, 0f, ));
		}

		public List<Pose2D> ref_poses = new List<Pose2D>();
	}
}

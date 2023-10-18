using Stride.Core.Mathematics;

namespace Stride.Engine;
public static class CameraExtensions
{
	public static Vector3 GetWorldPosition(this CameraComponent camera)
	{
		return camera.Entity.Transform.LocalToWorld(camera.Entity.Transform.Position);
	}
}

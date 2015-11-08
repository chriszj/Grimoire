using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/* Keeps a reference of clone's eye transformation matrix for 
 * external model placements.
 * 
 * How much unity are in a cm ? -- Aprox 2 units are 1 cm
 * In position 1cm = 1unity and scale it depends on the content, but aparently 1cm = 2units of scale
 * 
 * Virtual objects are scaled according to the width of the eyes.
 *  
 * Test Mauricio 
 * Eye Width = 1cm 
 * 
 * Pending? - rotation. Most the time wont be used but would be 
 * 
 */

namespace GLIB.Utils {

	public static class MetricsTransform {

		static Vector3 _virtual3DPos = new Vector3();
		public static Vector3 virtual3DPos{ get{ return _virtual3DPos; } } 

		static Vector3 _virtual3DRotation = new Vector3();
		public static Vector3 virtual3DRotation{ get{return _virtual3DRotation;} }

		static Vector3 _virtual3DScale = new Vector3(1, 1, 1);
		public static Vector3 virtual3DScale{ get{return _virtual3DScale; } }

		public const float _scaleUnityUnitsPerCentimeter = 2;
		public const float _posUnityUnitsPerCentimeter = 10;

		static Matrix4x4 _transfMatrix = new Matrix4x4();

		/// <summary>
		/// Set the physical position, rotation and dimmensions of an object to be used for transforming vectors 
		/// </summary>
		/// <param name="eye_cmwidth">The eye physical width in centimeters</param>
		/// <param name="eye_cmposition">The eye physical tridimensional position in centimeters.</param>
		public static void SetCMPhysicalTransform(Vector3 cm_basePosition = new Vector3(), Vector3 baseEulerAngles = new Vector3(), Vector3 cm_baseDimmensions = new Vector3())
		{
			_virtual3DPos = new Vector3(cm_basePosition.x * _posUnityUnitsPerCentimeter, cm_basePosition.y * _posUnityUnitsPerCentimeter, cm_basePosition.z * _posUnityUnitsPerCentimeter);

			_virtual3DScale = new Vector3(cm_baseDimmensions.x * _scaleUnityUnitsPerCentimeter, cm_baseDimmensions.y * _scaleUnityUnitsPerCentimeter, cm_baseDimmensions.z * _scaleUnityUnitsPerCentimeter);

			_virtual3DRotation = baseEulerAngles;

		}

		/// <summary>
		/// Transform a given Vector3 with the matrix transform of the base, useful for 
		/// </summary>
		/// <returns>The vector with the transformation applied.</returns>
		/// <param name="vector">The vector that is going to be transformed using clone data matrix transform.</param>
		/*public static Vector3 TransformVector3(Vector3 vector){

			_transfMatrix.SetTRS(_virtual3DPos, Quaternion.Euler(_virtual3DRotation), _virtual3DScale);
			vector = _transfMatrix.MultiplyPoint3x4(vector);

			return vector;

		}*/

		/// <summary>
		/// Transform a given Vector3 in Centimeters and returns a transformed vector according to the unitsPerCentimeter factor 
		/// </summary>
		/// <returns>The vector3.</returns>
		/// <param name="vector">A Vector3 in centimeters.</param>
		/// <param name="unitsPerCM">Units per Centimeters factor to be used as the transformation parameter</param>
		public static Vector3 TransformVector3(Vector3 vector, float unitsPerCM){

			vector = new Vector3(vector.x * unitsPerCM, vector.y * unitsPerCM, vector.z * unitsPerCM);

			return vector;

		}
	}

}

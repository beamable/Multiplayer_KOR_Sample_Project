using System;
using System.Runtime.CompilerServices;
using static UnityS.Mathematics.math;

namespace UnityS.Mathematics
{
    [Serializable]
    public struct RigidTransform
    {
        public quaternion rot;
        public float3 pos;

        /// <summary>A RigidTransform representing the identity transform.</summary>
        public static readonly RigidTransform identity = new RigidTransform(new quaternion(sfloat.Zero, sfloat.Zero, sfloat.Zero, sfloat.One), new float3(sfloat.Zero, sfloat.Zero, sfloat.Zero));

        /// <summary>Constructs a RigidTransform from a rotation represented by a unit quaternion and a translation represented by a float3 vector.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RigidTransform(quaternion rotation, float3 translation)
        {
            this.rot = rotation;
            this.pos = translation;
        }

        /// <summary>Constructs a RigidTransform from a rotation represented by a float3x3 matrix and a translation represented by a float3 vector.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RigidTransform(float3x3 rotation, float3 translation)
        {
            this.rot = new quaternion(rotation);
            this.pos = translation;
        }

        /// <summary>Constructs a RigidTransform from a float4x4. Assumes the matrix is orthonormal.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RigidTransform(float4x4 transform)
        {
            this.rot = new quaternion(transform);
            this.pos = transform.c3.xyz;
        }


        /// <summary>
        /// Returns a RigidTransform representing a rotation around a unit axis by an angle in radians.
        /// The rotation direction is clockwise when looking along the rotation axis towards the origin.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform AxisAngle(float3 axis, sfloat angle) { return new RigidTransform(quaternion.AxisAngle(axis, angle), float3.zero); }

        /// <summary>
        /// Returns a RigidTransform constructed by first performing a rotation around the x-axis, then the y-axis and finally the z-axis.
        /// All rotation angles are in radians and clockwise when looking along the rotation axis towards the origin.
        /// </summary>
        /// <param name="xyz">A float3 vector containing the rotation angles around the x-, y- and z-axis measures in radians.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform EulerXYZ(float3 xyz) { return new RigidTransform(quaternion.EulerXYZ(xyz), float3.zero); }

        /// <summary>
        /// Returns a RigidTransform constructed by first performing a rotation around the x-axis, then the z-axis and finally the y-axis.
        /// All rotation angles are in radians and clockwise when looking along the rotation axis towards the origin.
        /// </summary>
        /// <param name="xyz">A float3 vector containing the rotation angles around the x-, y- and z-axis measures in radians.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform EulerXZY(float3 xyz) { return new RigidTransform(quaternion.EulerXZY(xyz), float3.zero); }

        /// <summary>
        /// Returns a RigidTransform constructed by first performing a rotation around the y-axis, then the x-axis and finally the z-axis.
        /// All rotation angles are in radians and clockwise when looking along the rotation axis towards the origin.
        /// </summary>
        /// <param name="xyz">A float3 vector containing the rotation angles around the x-, y- and z-axis measures in radians.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform EulerYXZ(float3 xyz) { return new RigidTransform(quaternion.EulerYXZ(xyz), float3.zero); }

        /// <summary>
        /// Returns a RigidTransform constructed by first performing a rotation around the y-axis, then the z-axis and finally the x-axis.
        /// All rotation angles are in radians and clockwise when looking along the rotation axis towards the origin.
        /// </summary>
        /// <param name="xyz">A float3 vector containing the rotation angles around the x-, y- and z-axis measures in radians.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform EulerYZX(float3 xyz) { return new RigidTransform(quaternion.EulerYZX(xyz), float3.zero); }


        /// <summary>
        /// Returns a RigidTransform constructed by first performing a rotation around the z-axis, then the x-axis and finally the y-axis.
        /// All rotation angles are in radians and clockwise when looking along the rotation axis towards the origin.
        /// This is the default order rotation order in Unity.
        /// </summary>
        /// <param name="xyz">A float3 vector containing the rotation angles around the x-, y- and z-axis measures in radians.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform EulerZXY(float3 xyz) { return new RigidTransform(quaternion.EulerZXY(xyz), float3.zero); }

        /// <summary>
        /// Returns a RigidTransform constructed by first performing a rotation around the z-axis, then the y-axis and finally the x-axis.
        /// All rotation angles are in radians and clockwise when looking along the rotation axis towards the origin.
        /// </summary>
        /// <param name="xyz">A float3 vector containing the rotation angles around the x-, y- and z-axis measures in radians.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform EulerZYX(float3 xyz) { return new RigidTransform(quaternion.EulerZYX(xyz), float3.zero); }

        /// <summary>
        /// Returns a RigidTransform constructed by first performing a rotation around the x-axis, then the y-axis and finally the z-axis.
        /// All rotation angles are in radians and clockwise when looking along the rotation axis towards the origin.
        /// </summary>
        /// <param name="x">The rotation angle around the x-axis in radians.</param>
        /// <param name="y">The rotation angle around the y-axis in radians.</param>
        /// <param name="z">The rotation angle around the z-axis in radians.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform EulerXYZ(sfloat x, sfloat y, sfloat z) { return EulerXYZ(float3(x, y, z)); }

        /// <summary>
        /// Returns a RigidTransform constructed by first performing a rotation around the x-axis, then the z-axis and finally the y-axis.
        /// All rotation angles are in radians and clockwise when looking along the rotation axis towards the origin.
        /// </summary>
        /// <param name="x">The rotation angle around the x-axis in radians.</param>
        /// <param name="y">The rotation angle around the y-axis in radians.</param>
        /// <param name="z">The rotation angle around the z-axis in radians.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform EulerXZY(sfloat x, sfloat y, sfloat z) { return EulerXZY(float3(x, y, z)); }

        /// <summary>
        /// Returns a RigidTransform constructed by first performing a rotation around the y-axis, then the x-axis and finally the z-axis.
        /// All rotation angles are in radians and clockwise when looking along the rotation axis towards the origin.
        /// </summary>
        /// <param name="x">The rotation angle around the x-axis in radians.</param>
        /// <param name="y">The rotation angle around the y-axis in radians.</param>
        /// <param name="z">The rotation angle around the z-axis in radians.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform EulerYXZ(sfloat x, sfloat y, sfloat z) { return EulerYXZ(float3(x, y, z)); }

        /// <summary>
        /// Returns a RigidTransform constructed by first performing a rotation around the y-axis, then the z-axis and finally the x-axis.
        /// All rotation angles are in radians and clockwise when looking along the rotation axis towards the origin.
        /// </summary>
        /// <param name="x">The rotation angle around the x-axis in radians.</param>
        /// <param name="y">The rotation angle around the y-axis in radians.</param>
        /// <param name="z">The rotation angle around the z-axis in radians.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform EulerYZX(sfloat x, sfloat y, sfloat z) { return EulerYZX(float3(x, y, z)); }

        /// <summary>
        /// Returns a RigidTransform constructed by first performing a rotation around the z-axis, then the x-axis and finally the y-axis.
        /// All rotation angles are in radians and clockwise when looking along the rotation axis towards the origin.
        /// This is the default order rotation order in Unity.
        /// </summary>
        /// <param name="x">The rotation angle around the x-axis in radians.</param>
        /// <param name="y">The rotation angle around the y-axis in radians.</param>
        /// <param name="z">The rotation angle around the z-axis in radians.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform EulerZXY(sfloat x, sfloat y, sfloat z) { return EulerZXY(float3(x, y, z)); }

        /// <summary>
        /// Returns a RigidTransform constructed by first performing a rotation around the z-axis, then the y-axis and finally the x-axis.
        /// All rotation angles are in radians and clockwise when looking along the rotation axis towards the origin.
        /// </summary>
        /// <param name="x">The rotation angle around the x-axis in radians.</param>
        /// <param name="y">The rotation angle around the y-axis in radians.</param>
        /// <param name="z">The rotation angle around the z-axis in radians.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform EulerZYX(sfloat x, sfloat y, sfloat z) { return EulerZYX(float3(x, y, z)); }

        /// <summary>
        /// Returns a RigidTransform constructed by first performing 3 rotations around the principal axes in a given order.
        /// All rotation angles are in radians and clockwise when looking along the rotation axis towards the origin.
        /// When the rotation order is known at compile time, it is recommended for performance reasons to use specific
        /// Euler rotation constructors such as EulerZXY(...).
        /// </summary>
        /// <param name="xyz">A float3 vector containing the rotation angles around the x-, y- and z-axis measures in radians.</param>
        /// <param name="order">The order in which the rotations are applied.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform Euler(float3 xyz, RotationOrder order = RotationOrder.ZXY)
        {
            switch (order)
            {
                case RotationOrder.XYZ:
                    return EulerXYZ(xyz);
                case RotationOrder.XZY:
                    return EulerXZY(xyz);
                case RotationOrder.YXZ:
                    return EulerYXZ(xyz);
                case RotationOrder.YZX:
                    return EulerYZX(xyz);
                case RotationOrder.ZXY:
                    return EulerZXY(xyz);
                case RotationOrder.ZYX:
                    return EulerZYX(xyz);
                default:
                    return RigidTransform.identity;
            }
        }

        /// <summary>
        /// Returns a RigidTransform constructed by first performing 3 rotations around the principal axes in a given order.
        /// All rotation angles are in radians and clockwise when looking along the rotation axis towards the origin.
        /// When the rotation order is known at compile time, it is recommended for performance reasons to use specific
        /// Euler rotation constructors such as EulerZXY(...).
        /// </summary>
        /// <param name="x">The rotation angle around the x-axis in radians.</param>
        /// <param name="y">The rotation angle around the y-axis in radians.</param>
        /// <param name="z">The rotation angle around the z-axis in radians.</param>
        /// <param name="order">The order in which the rotations are applied.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform Euler(sfloat x, sfloat y, sfloat z, RotationOrder order = RotationOrder.Default)
        {
            return Euler(float3(x, y, z), order);
        }

        /// <summary>Returns a float4x4 matrix that rotates around the x-axis by a given number of radians.</summary>
        /// <param name="angle">The clockwise rotation angle when looking along the x-axis towards the origin in radians.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform RotateX(sfloat angle)
        {
            return new RigidTransform(quaternion.RotateX(angle), float3.zero);
        }

        /// <summary>Returns a float4x4 matrix that rotates around the y-axis by a given number of radians.</summary>
        /// <param name="angle">The clockwise rotation angle when looking along the y-axis towards the origin in radians.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform RotateY(sfloat angle)
        {
            return new RigidTransform(quaternion.RotateY(angle), float3.zero);
        }

        /// <summary>Returns a float4x4 matrix that rotates around the z-axis by a given number of radians.</summary>
        /// <param name="angle">The clockwise rotation angle when looking along the z-axis towards the origin in radians.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform RotateZ(sfloat angle)
        {
            return new RigidTransform(quaternion.RotateZ(angle), float3.zero);
        }

        /// <summary>Returns a RigidTransform that translates by an amount specified by a float3 vector.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform Translate(float3 vector)
        {
            return new RigidTransform(quaternion.identity, vector);
        }


        /// <summary>Returns true if the RigidTransform is equal to a given RigidTransform, false otherwise.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(RigidTransform x) { return rot.Equals(x.rot) && pos.Equals(x.pos); }

        /// <summary>Returns true if the RigidTransform is equal to a given RigidTransform, false otherwise.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object x) { return Equals((RigidTransform)x); }

        /// <summary>Returns a hash code for the RigidTransform.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() { return (int)math.hash(this); }

        /// <summary>Returns a string representation of the RigidTransform.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return string.Format("RigidTransform(({0}f, {1}f, {2}f, {3}f),  ({4}f, {5}f, {6}f))",
                rot.value.x, rot.value.y, rot.value.z, rot.value.w, pos.x, pos.y, pos.z);
        }

        /// <summary>Returns a string representation of the quaternion using a specified format and culture-specific format information.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format("float4x4(({0}f, {1}f, {2}f, {3}f),  ({4}f, {5}f, {6}f))",
                rot.value.x.ToString(format, formatProvider),
                rot.value.y.ToString(format, formatProvider),
                rot.value.z.ToString(format, formatProvider),
                rot.value.w.ToString(format, formatProvider),
                pos.x.ToString(format, formatProvider),
                pos.y.ToString(format, formatProvider),
                pos.z.ToString(format, formatProvider));
        }
    }

    public static partial class math
    {
        /// <summary>Returns a RigidTransform constructed from a rotation represented by a unit quaternion and a translation represented by a float3 vector.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform RigidTransform(quaternion rot, float3 pos) { return new RigidTransform(rot, pos); }

        /// <summary>Returns a RigidTransform constructed from a rotation represented by a unit quaternion and a translation represented by a float3 vector.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform RigidTransform(float3x3 rotation, float3 translation) { return new RigidTransform(rotation, translation); }

        /// <summary>Returns a RigidTransform constructed from a rotation represented by a float3x3 matrix and a translation represented by a float3 vector.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform RigidTransform(float4x4 transform) { return new RigidTransform(transform); }

        /// <summary>Returns the inverse of a RigidTransform.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform inverse(RigidTransform t)
        {
            quaternion invRotation = inverse(t.rot);
            float3 invTranslation = mul(invRotation, -t.pos);
            return new RigidTransform(invRotation, invTranslation);
        }

        /// <summary>Returns the result of transforming the RigidTransform b by the RigidTransform a.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RigidTransform mul(RigidTransform a, RigidTransform b)
        {
            return new RigidTransform(mul(a.rot, b.rot), mul(a.rot, b.pos) + a.pos);
        }

        /// <summary>Returns the result of transforming a float4 homogeneous coordinate by a RigidTransform.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 mul(RigidTransform a, float4 pos)
        {
            return float4(mul(a.rot, pos.xyz) + a.pos * pos.w, pos.w);
        }

        /// <summary>Returns the result of rotating a float3 vector by a RigidTransform.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 rotate(RigidTransform a, float3 dir)
        {
            return mul(a.rot, dir);
        }

        /// <summary>Returns the result of transforming a float3 point by a RigidTransform.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 transform(RigidTransform a, float3 pos)
        {
            return mul(a.rot, pos) + a.pos;
        }

        /// <summary>Returns a uint hash code of a RigidTransform.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(RigidTransform t)
        {
            return hash(t.rot) + 0xC5C5394Bu * hash(t.pos);
        }

        /// <summary>
        /// Returns a uint4 vector hash code of a RigidTransform.
        /// When multiple elements are to be hashes together, it can more efficient to calculate and combine wide hash
        /// that are only reduced to a narrow uint hash at the very end instead of at every step.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 hashwide(RigidTransform t)
        {
            return hashwide(t.rot) + 0xC5C5394Bu * hashwide(t.pos).xyzz;
        }
    }
}

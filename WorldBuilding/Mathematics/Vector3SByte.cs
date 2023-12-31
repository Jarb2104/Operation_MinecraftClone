﻿// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
//
// -----------------------------------------------------------------------------
// Original code from SlimMath project. http://code.google.com/p/slimmath/
// Greetings to SlimDX Group. Original code published with the following license:
// -----------------------------------------------------------------------------
/*
* Copyright (c) 2007-2011 SlimDX Group
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using Stride.Core;
using Stride.Core.Mathematics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WorldBuilding.Mathematics
{
    /// <summary>
    /// Represents a three dimensional mathematical vector.
    /// </summary>
    [DataContract("sbyte3")]
    [DataStyle(DataStyle.Compact)]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vector3SByte : IEquatable<Vector3SByte>, IFormattable
    {
        /// <summary>
        /// The size of the <see cref="Vector3SByte"/> type, in sbytes.
        /// </summary>
        public static readonly int SizeInBytes = Unsafe.SizeOf<Vector3SByte>();

        /// <summary>
        /// A <see cref="Vector3SByte"/> with all of its components set to zero.
        /// </summary>
        public static readonly Vector3SByte Zero = new();

        /// <summary>
        /// The X unit <see cref="Vector3SByte"/> (1, 0, 0).
        /// </summary>
        public static readonly Vector3SByte UnitX = new(1, 0, 0);

        /// <summary>
        /// The Y unit <see cref="Vector3SByte"/> (0, 1, 0).
        /// </summary>
        public static readonly Vector3SByte UnitY = new(0, 1, 0);

        /// <summary>
        /// The Z unit <see cref="Vector3SByte"/> (0, 0, 1).
        /// </summary>
        public static readonly Vector3SByte UnitZ = new(0, 0, 1);

        /// <summary>
        /// A <see cref="Vector3SByte"/> with all of its components set to one.
        /// </summary>
        public static readonly Vector3SByte One = new(1, 1, 1);

        /// <summary>
        /// The X component of the vector.
        /// </summary>
        [DataMember(0)]
        public sbyte X;

        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        [DataMember(1)]
        public sbyte Y;

        /// <summary>
        /// The Z component of the vector.
        /// </summary>
        [DataMember(2)]
        public sbyte Z;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3SByte"/> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public Vector3SByte()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3SByte"/> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public Vector3SByte(sbyte value)
        {
            X = value;
            Y = value;
            Z = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3SByte"/> struct.
        /// </summary>
        /// <param name="x">Initial value for the X component of the vector.</param>
        /// <param name="y">Initial value for the Y component of the vector.</param>
        /// <param name="z">Initial value for the Z component of the vector.</param>
        public Vector3SByte(sbyte x, sbyte y, sbyte z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3SByte"/> struct.
        /// </summary>
        /// <param name="values">The values to assign to the X, Y, and Z components of the vector. This must be an array with three elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than three elements.</exception>
        public Vector3SByte(sbyte[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length != 3)
                throw new ArgumentOutOfRangeException(nameof(values), "There must be three and only three input values for Vector3SByte.");

            X = values[0];
            Y = values[1];
            Z = values[2];
        }

        /// <summary>
        /// Gets a value indicting whether this instance is normalized.
        /// </summary>
        public readonly bool IsNormalized
        {
            get { return MathF.Abs((X * X) + (Y * Y) + (Z * Z) - 1) < MathUtil.ZeroTolerance; }
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the X, Y, or Z component, depending on the index.</value>
        /// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component, and 2 for the Z component.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 2].</exception>
        public sbyte this[sbyte index]
        {
            readonly get
            {
                return index switch
                {
                    0 => X,
                    1 => Y,
                    2 => Z,
                    _ => throw new ArgumentOutOfRangeException(nameof(index), "Indices for Vector3SByte run from 0 to 2, inclusive."),
                };
                throw new ArgumentOutOfRangeException(nameof(index), "Indices for Vector3SByte run from 0 to 2, inclusive.");
            }

            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index), "Indices for Vector3SByte run from 0 to 2, inclusive.");
                }
            }
        }

        /// <summary>
        /// Calculates the length of the vector.
        /// </summary>
        /// <returns>The length of the vector.</returns>
        /// <remarks>
        /// <see cref="Vector3SByte.LengthSquared"/> may be preferred when only the relative length is needed
        /// and speed is of the essence.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly sbyte Length()
        {
            return (sbyte)MathF.Sqrt((X * X) + (Y * Y) + (Z * Z));
        }

        /// <summary>
        /// Calculates the squared length of the vector.
        /// </summary>
        /// <returns>The squared length of the vector.</returns>
        /// <remarks>
        /// This method may be preferred to <see cref="Vector3SByte.Length"/> when only a relative length is needed
        /// and speed is of the essence.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly sbyte LengthSquared()
        {
            return (sbyte)((X * X) + (Y * Y) + (Z * Z));
        }

        /// <summary>
        /// Converts the vector into a unit vector.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            sbyte length = Length();
            if (length > MathUtil.ZeroTolerance)
            {
                sbyte inv = (sbyte)(1 / length);
                X *= inv;
                Y *= inv;
                Z *= inv;
            }
        }

        /// <summary>
        /// Raises the exponent for each components.
        /// </summary>
        /// <param name="exponent">The exponent.</param>
        public void Pow(sbyte exponent)
        {
            X = (sbyte)MathF.Pow(X, exponent);
            Y = (sbyte)MathF.Pow(Y, exponent);
            Z = (sbyte)MathF.Pow(Z, exponent);
        }

        /// <summary>
        /// Creates an array containing the elements of the vector.
        /// </summary>
        /// <returns>A three-element array containing the components of the vector.</returns>
        public readonly sbyte[] ToArray()
        {
            return new sbyte[] { X, Y, Z };
        }

        /// <summary>
        /// Moves the first Vector3SByte to the second one in a straight line.
        /// </summary>
        /// <param name="from">The first point.</param>
        /// <param name="to">The second point.</param>
        /// <param name="maxTravelDistance">The rate at which the first point is going to move towards the second point.</param>
        public static Vector3SByte MoveTo(in Vector3SByte from, in Vector3SByte to, sbyte maxTravelDistance)
        {
            Vector3SByte distance = Subtract(to, from);

            sbyte length = distance.Length();

            if (maxTravelDistance >= length || length == 0)
                return to;
            else
                return new Vector3SByte((sbyte)(from.X + distance.X / length * maxTravelDistance), (sbyte)(from.Y + distance.Y / length * maxTravelDistance), (sbyte)(from.Z + distance.Z / length * maxTravelDistance));
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="left">The first vector to add.</param>
        /// <param name="right">The second vector to add.</param>
        /// <param name="result">When the method completes, contains the sum of the two vectors.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(ref Vector3SByte left, ref Vector3SByte right, out Vector3SByte result)
        {
            result = new Vector3SByte((sbyte)(left.X + right.X), (sbyte)(left.Y + right.Y), (sbyte)(left.Z + right.Z));
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="left">The first vector to add.</param>
        /// <param name="right">The second vector to add.</param>
        /// <returns>The sum of the two vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte Add(Vector3SByte left, Vector3SByte right)
        {
            return new Vector3SByte((sbyte)(left.X + right.X), (sbyte)(left.Y + right.Y), (sbyte)(left.Z + right.Z));
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="left">The first vector to subtract.</param>
        /// <param name="right">The second vector to subtract.</param>
        /// <param name="result">When the method completes, contains the difference of the two vectors.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Subtract(ref Vector3SByte left, ref Vector3SByte right, out Vector3SByte result)
        {
            result = new Vector3SByte((sbyte)(left.X - right.X), (sbyte)(left.Y - right.Y), (sbyte)(left.Z - right.Z));
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="left">The first vector to subtract.</param>
        /// <param name="right">The second vector to subtract.</param>
        /// <returns>The difference of the two vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte Subtract(in Vector3SByte left, in Vector3SByte right)
        {
            return new Vector3SByte((sbyte)(left.X - right.X), (sbyte)(left.Y - right.Y), (sbyte)(left.Z - right.Z));
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scale">The amount by which to scale the vector.</param>
        /// <param name="result">When the method completes, contains the scaled vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(ref Vector3SByte value, sbyte scale, out Vector3SByte result)
        {
            result = new Vector3SByte((sbyte)(value.X * scale), (sbyte)(value.Y * scale), (sbyte)(value.Z * scale));
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scale">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte Multiply(Vector3SByte value, sbyte scale)
        {
            return new Vector3SByte((sbyte)(value.X * scale), (sbyte)(value.Y * scale), (sbyte)(value.Z * scale));
        }

        /// <summary>
        /// Modulates a vector with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">The first vector to modulate.</param>
        /// <param name="right">The second vector to modulate.</param>
        /// <param name="result">When the method completes, contains the modulated vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Modulate(ref Vector3SByte left, ref Vector3SByte right, out Vector3SByte result)
        {
            result = new Vector3SByte((sbyte)(left.X * right.X), (sbyte)(left.Y * right.Y), (sbyte)(left.Z * right.Z));
        }

        /// <summary>
        /// Modulates a vector with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">The first vector to modulate.</param>
        /// <param name="right">The second vector to modulate.</param>
        /// <returns>The modulated vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte Modulate(Vector3SByte left, Vector3SByte right)
        {
            return new Vector3SByte((sbyte)(left.X * right.X), (sbyte)(left.Y * right.Y), (sbyte)(left.Z * right.Z));
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scale">The amount by which to scale the vector.</param>
        /// <param name="result">When the method completes, contains the scaled vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Divide(ref Vector3SByte value, sbyte scale, out Vector3SByte result)
        {
            result = new Vector3SByte((sbyte)(value.X / scale), (sbyte)(value.Y / scale), (sbyte)(value.Z / scale));
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scale">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte Divide(Vector3SByte value, sbyte scale)
        {
            return new Vector3SByte((sbyte)(value.X / scale), (sbyte)(value.Y / scale), (sbyte)(value.Z / scale));
        }

        /// <summary>
        /// Demodulates a vector with another by performing component-wise division.
        /// </summary>
        /// <param name="left">The first vector to demodulate.</param>
        /// <param name="right">The second vector to demodulate.</param>
        /// <param name="result">When the method completes, contains the demodulated vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Demodulate(ref Vector3SByte left, ref Vector3SByte right, out Vector3SByte result)
        {
            result = new Vector3SByte((sbyte)(left.X / right.X), (sbyte)(left.Y / right.Y), (sbyte)(left.Z / right.Z));
        }

        /// <summary>
        /// Demodulates a vector with another by performing component-wise division.
        /// </summary>
        /// <param name="left">The first vector to demodulate.</param>
        /// <param name="right">The second vector to demodulate.</param>
        /// <returns>The demodulated vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte Demodulate(Vector3SByte left, Vector3SByte right)
        {
            return new Vector3SByte((sbyte)(left.X / right.X), (sbyte)(left.Y / right.Y), (sbyte)(left.Z / right.Z));
        }

        /// <summary>
        /// Reverses the direction of a given vector.
        /// </summary>
        /// <param name="value">The vector to negate.</param>
        /// <param name="result">When the method completes, contains a vector facing in the opposite direction.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Negate(ref Vector3SByte value, out Vector3SByte result)
        {
            result = new Vector3SByte((sbyte)(-value.X), (sbyte)(-value.Y), (sbyte)(-value.Z));
        }

        /// <summary>
        /// Reverses the direction of a given vector.
        /// </summary>
        /// <param name="value">The vector to negate.</param>
        /// <returns>A vector facing in the opposite direction.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte Negate(Vector3SByte value)
        {
            return new Vector3SByte((sbyte)(-value.X), (sbyte)(-value.Y), (sbyte)(-value.Z));
        }

        /// <summary>
        /// Returns a <see cref="Vector3SByte"/> containing the 3D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 3D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="Vector3SByte"/> containing the 3D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="Vector3SByte"/> containing the 3D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="Vector3SByte"/> containing the 3D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        /// <param name="result">When the method completes, contains the 3D Cartesian coordinates of the specified point.</param>
        public static void Barycentric(ref Vector3SByte value1, ref Vector3SByte value2, ref Vector3SByte value3, sbyte amount1, sbyte amount2, out Vector3SByte result)
        {
            result = new Vector3SByte(
                (sbyte)((value1.X + (amount1 * (value2.X - value1.X))) + (amount2 * (value3.X - value1.X))),
                (sbyte)((value1.Y + (amount1 * (value2.Y - value1.Y))) + (amount2 * (value3.Y - value1.Y))),
                (sbyte)((value1.Z + (amount1 * (value2.Z - value1.Z))) + (amount2 * (value3.Z - value1.Z))));
        }

        /// <summary>
        /// Returns a <see cref="Vector3SByte"/> containing the 3D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 3D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="Vector3SByte"/> containing the 3D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="Vector3SByte"/> containing the 3D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="Vector3SByte"/> containing the 3D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        /// <returns>A new <see cref="Vector3SByte"/> containing the 3D Cartesian coordinates of the specified point.</returns>
        public static Vector3SByte Barycentric(Vector3SByte value1, Vector3SByte value2, Vector3SByte value3, sbyte amount1, sbyte amount2)
        {
            Barycentric(ref value1, ref value2, ref value3, amount1, amount2, out Vector3SByte result);
            return result;
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="result">When the method completes, contains the clamped value.</param>
        public static void Clamp(ref Vector3SByte value, ref Vector3SByte min, ref Vector3SByte max, out Vector3SByte result)
        {
            sbyte x = value.X;
            x = (x > max.X) ? max.X : x;
            x = (x < min.X) ? min.X : x;

            sbyte y = value.Y;
            y = (y > max.Y) ? max.Y : y;
            y = (y < min.Y) ? min.Y : y;

            sbyte z = value.Z;
            z = (z > max.Z) ? max.Z : z;
            z = (z < min.Z) ? min.Z : z;

            result = new Vector3SByte(x, y, z);
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The clamped value.</returns>
        public static Vector3SByte Clamp(Vector3SByte value, Vector3SByte min, Vector3SByte max)
        {
            Clamp(ref value, ref min, ref max, out Vector3SByte result);
            return result;
        }

        /// <summary>
        /// Calculates the cross product of two vectors.
        /// </summary>
        /// <param name="left">First source vector.</param>
        /// <param name="right">Second source vector.</param>
        /// <param name="result">When the method completes, contains he cross product of the two vectors.</param>
        public static void Cross(ref Vector3SByte left, ref Vector3SByte right, out Vector3SByte result)
        {
            result = new Vector3SByte(
                (sbyte)((left.Y * right.Z) - (left.Z * right.Y)),
                (sbyte)((left.Z * right.X) - (left.X * right.Z)),
                (sbyte)((left.X * right.Y) - (left.Y * right.X)));
        }

        /// <summary>
        /// Calculates the cross product of two vectors.
        /// </summary>
        /// <param name="left">First source vector.</param>
        /// <param name="right">Second source vector.</param>
        /// <returns>The cross product of the two vectors.</returns>
        public static Vector3SByte Cross(in Vector3SByte left, in Vector3SByte right)
        {
            return new Vector3SByte(
                (sbyte)((left.Y * right.Z) - (left.Z * right.Y)),
                (sbyte)((left.Z * right.X) - (left.X * right.Z)),
                (sbyte)((left.X * right.Y) - (left.Y * right.X)));
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="result">When the method completes, contains the distance between the two vectors.</param>
        /// <remarks>
        /// <see cref="Vector3SByte.DistanceSquared(ref Vector3SByte, ref Vector3SByte, out sbyte)"/> may be preferred when only the relative distance is needed
        /// and speed is of the essence.
        /// </remarks>
        public static void Distance(ref Vector3SByte value1, ref Vector3SByte value2, out sbyte result)
        {
            sbyte x = (sbyte)(value1.X - value2.X);
            sbyte y = (sbyte)(value1.Y - value2.Y);
            sbyte z = (sbyte)(value1.Z - value2.Z);

            result = (sbyte)MathF.Sqrt((x * x) + (y * y) + (z * z));
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The distance between the two vectors.</returns>
        /// <remarks>
        /// <see cref="Vector3SByte.DistanceSquared(Vector3SByte, Vector3SByte)"/> may be preferred when only the relative distance is needed
        /// and speed is of the essence.
        /// </remarks>
        public static sbyte Distance(Vector3SByte value1, Vector3SByte value2)
        {
            sbyte x = (sbyte)(value1.X - value2.X);
            sbyte y = (sbyte)(value1.Y - value2.Y);
            sbyte z = (sbyte)(value1.Z - value2.Z);

            return (sbyte)MathF.Sqrt((x * x) + (y * y) + (z * z));
        }

        /// <summary>
        /// Calculates the squared distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="result">When the method completes, contains the squared distance between the two vectors.</param>
        /// <remarks>Distance squared is the value before taking the square root. 
        /// Distance squared can often be used in place of distance if relative comparisons are being made. 
        /// For example, consider three points A, B, and C. To determine whether B or C is further from A, 
        /// compare the distance between A and B to the distance between A and C. Calculating the two distances 
        /// involves two square roots, which are computationally expensive. However, using distance squared 
        /// provides the same information and avoids calculating two square roots.
        /// </remarks>
        public static void DistanceSquared(ref Vector3SByte value1, ref Vector3SByte value2, out sbyte result)
        {
            sbyte x = (sbyte)(value1.X - value2.X);
            sbyte y = (sbyte)(value1.Y - value2.Y);
            sbyte z = (sbyte)(value1.Z - value2.Z);

            result = (sbyte)((x * x) + (y * y) + (z * z));
        }

        /// <summary>
        /// Calculates the squared distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The squared distance between the two vectors.</returns>
        /// <remarks>Distance squared is the value before taking the square root. 
        /// Distance squared can often be used in place of distance if relative comparisons are being made. 
        /// For example, consider three points A, B, and C. To determine whether B or C is further from A, 
        /// compare the distance between A and B to the distance between A and C. Calculating the two distances 
        /// involves two square roots, which are computationally expensive. However, using distance squared 
        /// provides the same information and avoids calculating two square roots.
        /// </remarks>
        public static sbyte DistanceSquared(Vector3SByte value1, Vector3SByte value2)
        {
            sbyte x = (sbyte)(value1.X - value2.X);
            sbyte y = (sbyte)(value1.Y - value2.Y);
            sbyte z = (sbyte)(value1.Z - value2.Z);

            return (sbyte)((x * x) + (y * y) + (z * z));
        }

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="left">First source vector.</param>
        /// <param name="right">Second source vector.</param>
        /// <param name="result">When the method completes, contains the dot product of the two vectors.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dot(ref Vector3SByte left, ref Vector3SByte right, out short result)
        {
            result = (short)((left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z));
        }

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="left">First source vector.</param>
        /// <param name="right">Second source vector.</param>
        /// <returns>The dot product of the two vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte Dot(Vector3SByte left, Vector3SByte right)
        {
            return (sbyte)((left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z));
        }

        /// <summary>
        /// Converts the vector into a unit vector.
        /// </summary>
        /// <param name="value">The vector to normalize.</param>
        /// <param name="result">When the method completes, contains the normalized vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Normalize(ref Vector3SByte value, out Vector3SByte result)
        {
            result = value;
            result.Normalize();
        }

        /// <summary>
        /// Converts the vector into a unit vector.
        /// </summary>
        /// <param name="value">The vector to normalize.</param>
        /// <returns>The normalized vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte Normalize(Vector3SByte value)
        {
            value.Normalize();
            return value;
        }

        /// <summary>
        /// Performs a linear interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the linear interpolation of the two vectors.</param>
        /// <remarks>
        /// This method performs the linear interpolation based on the following formula.
        /// <code>start + (end - start) * amount</code>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static void Lerp(ref Vector3SByte start, ref Vector3SByte end, sbyte amount, out Vector3SByte result)
        {
            result.X = (sbyte)(start.X + ((end.X - start.X) * amount));
            result.Y = (sbyte)(start.Y + ((end.Y - start.Y) * amount));
            result.Z = (sbyte)(start.Z + ((end.Z - start.Z) * amount));
        }

        /// <summary>
        /// Performs a linear interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>The linear interpolation of the two vectors.</returns>
        /// <remarks>
        /// This method performs the linear interpolation based on the following formula.
        /// <code>start + (end - start) * amount</code>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static Vector3SByte Lerp(Vector3SByte start, Vector3SByte end, sbyte amount)
        {
            Lerp(ref start, ref end, amount, out Vector3SByte result);
            return result;
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the cubic interpolation of the two vectors.</param>
        public static void SmoothStep(ref Vector3SByte start, ref Vector3SByte end, sbyte amount, out Vector3SByte result)
        {
            amount = (sbyte)((amount > 1) ? 1 : ((amount < 0) ? 0 : amount));
            amount = (sbyte)((amount * amount) * (3 - (2 * amount)));

            result.X = (sbyte)(start.X + ((end.X - start.X) * amount));
            result.Y = (sbyte)(start.Y + ((end.Y - start.Y) * amount));
            result.Z = (sbyte)(start.Z + ((end.Z - start.Z) * amount));
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>The cubic interpolation of the two vectors.</returns>
        public static Vector3SByte SmoothStep(Vector3SByte start, Vector3SByte end, sbyte amount)
        {
            SmoothStep(ref start, ref end, amount, out Vector3SByte result);
            return result;
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position vector.</param>
        /// <param name="tangent1">First source tangent vector.</param>
        /// <param name="value2">Second source position vector.</param>
        /// <param name="tangent2">Second source tangent vector.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <param name="result">When the method completes, contains the result of the Hermite spline interpolation.</param>
        public static void Hermite(ref Vector3SByte value1, ref Vector3SByte tangent1, ref Vector3SByte value2, ref Vector3SByte tangent2, sbyte amount, out Vector3SByte result)
        {
            sbyte squared = (sbyte)(amount * amount);
            sbyte cubed = (sbyte)(amount * squared);
            sbyte part1 = (sbyte)(((2 * cubed) - (3 * squared)) + 1);
            sbyte part2 = (sbyte)((-2 * cubed) + (3 * squared));
            sbyte part3 = (sbyte)((cubed - (2 * squared)) + amount);
            sbyte part4 = (sbyte)(cubed - squared);

            result.X = (sbyte)((((value1.X * part1) + (value2.X * part2)) + (tangent1.X * part3)) + (tangent2.X * part4));
            result.Y = (sbyte)((((value1.Y * part1) + (value2.Y * part2)) + (tangent1.Y * part3)) + (tangent2.Y * part4));
            result.Z = (sbyte)((((value1.Z * part1) + (value2.Z * part2)) + (tangent1.Z * part3)) + (tangent2.Z * part4));
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position vector.</param>
        /// <param name="tangent1">First source tangent vector.</param>
        /// <param name="value2">Second source position vector.</param>
        /// <param name="tangent2">Second source tangent vector.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The result of the Hermite spline interpolation.</returns>
        public static Vector3SByte Hermite(Vector3SByte value1, Vector3SByte tangent1, Vector3SByte value2, Vector3SByte tangent2, sbyte amount)
        {
            Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out Vector3SByte result);
            return result;
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="value1">The first position in the interpolation.</param>
        /// <param name="value2">The second position in the interpolation.</param>
        /// <param name="value3">The third position in the interpolation.</param>
        /// <param name="value4">The fourth position in the interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <param name="result">When the method completes, contains the result of the Catmull-Rom interpolation.</param>
        public static void CatmullRom(ref Vector3SByte value1, ref Vector3SByte value2, ref Vector3SByte value3, ref Vector3SByte value4, sbyte amount, out Vector3SByte result)
        {
            sbyte squared = (sbyte)(amount * amount);
            sbyte cubed = (sbyte)(amount * squared);

            result.X = (sbyte)(0.5 * ((((2 * value2.X) + ((-value1.X + value3.X) * amount)) +
            (((((2 * value1.X) - (5 * value2.X)) + (4 * value3.X)) - value4.X) * squared)) +
            ((((-value1.X + (3 * value2.X)) - (3 * value3.X)) + value4.X) * cubed)));

            result.Y = (sbyte)(0.5 * ((((2 * value2.Y) + ((-value1.Y + value3.Y) * amount)) +
                (((((2 * value1.Y) - (5 * value2.Y)) + (4 * value3.Y)) - value4.Y) * squared)) +
                ((((-value1.Y + (3 * value2.Y)) - (3 * value3.Y)) + value4.Y) * cubed)));

            result.Z = (sbyte)(0.5 * ((((2 * value2.Z) + ((-value1.Z + value3.Z) * amount)) +
                (((((2 * value1.Z) - (5 * value2.Z)) + (4 * value3.Z)) - value4.Z) * squared)) +
                ((((-value1.Z + (3 * value2.Z)) - (3 * value3.Z)) + value4.Z) * cubed)));
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="value1">The first position in the interpolation.</param>
        /// <param name="value2">The second position in the interpolation.</param>
        /// <param name="value3">The third position in the interpolation.</param>
        /// <param name="value4">The fourth position in the interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>A vector that is the result of the Catmull-Rom interpolation.</returns>
        public static Vector3SByte CatmullRom(Vector3SByte value1, Vector3SByte value2, Vector3SByte value3, Vector3SByte value4, sbyte amount)
        {
            CatmullRom(ref value1, ref value2, ref value3, ref value4, amount, out Vector3SByte result);
            return result;
        }

        /// <summary>
        /// Performs mathematical modulo component-wise (see MathUtil.Mod).
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <param name="result">When the method completes, contains an new vector composed of each component's modulo.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Mod(ref Vector3SByte left, ref Vector3SByte right, out Vector3SByte result)
        {
            result.X = (sbyte)MathUtil.Mod(left.X, right.X);
            result.Y = (sbyte)MathUtil.Mod(left.Y, right.Y);
            result.Z = (sbyte)MathUtil.Mod(left.Z, right.Z);
        }

        /// <summary>
        /// Performs mathematical modulo component-wise (see MathUtil.Mod).
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>When the method completes, contains an new vector composed of each component's modulo.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte Mod(Vector3SByte left, Vector3SByte right)
        {
            Mod(ref left, ref right, out Vector3SByte result);
            return result;
        }

        /// <summary>
        /// Returns a vector containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <param name="result">When the method completes, contains an new vector composed of the largest components of the source vectors.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(ref Vector3SByte left, ref Vector3SByte right, out Vector3SByte result)
        {
            result.X = (left.X > right.X) ? left.X : right.X;
            result.Y = (left.Y > right.Y) ? left.Y : right.Y;
            result.Z = (left.Z > right.Z) ? left.Z : right.Z;
        }

        /// <summary>
        /// Returns a vector containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>A vector containing the largest components of the source vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte Max(Vector3SByte left, Vector3SByte right)
        {
            Max(ref left, ref right, out Vector3SByte result);
            return result;
        }

        /// <summary>
        /// Returns a vector containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <param name="result">When the method completes, contains an new vector composed of the smallest components of the source vectors.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Min(ref Vector3SByte left, ref Vector3SByte right, out Vector3SByte result)
        {
            result.X = (left.X < right.X) ? left.X : right.X;
            result.Y = (left.Y < right.Y) ? left.Y : right.Y;
            result.Z = (left.Z < right.Z) ? left.Z : right.Z;
        }

        /// <summary>
        /// Returns a vector containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>A vector containing the smallest components of the source vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte Min(Vector3SByte left, Vector3SByte right)
        {
            Min(ref left, ref right, out Vector3SByte result);
            return result;
        }

        /// <summary>
        /// Projects a 3D vector from object space into screen space. 
        /// </summary>
        /// <param name="vector">The vector to project.</param>
        /// <param name="x">The X position of the viewport.</param>
        /// <param name="y">The Y position of the viewport.</param>
        /// <param name="width">The width of the viewport.</param>
        /// <param name="height">The height of the viewport.</param>
        /// <param name="minZ">The minimum depth of the viewport.</param>
        /// <param name="maxZ">The maximum depth of the viewport.</param>
        /// <param name="worldViewProjection">The combined world-view-projection matrix.</param>
        /// <param name="result">When the method completes, contains the vector in screen space.</param>
        public static void Project(ref Vector3SByte vector, sbyte x, sbyte y, sbyte width, sbyte height, sbyte minZ, sbyte maxZ, ref Matrix worldViewProjection, out Vector3SByte result)
        {
            TransformCoordinate(ref vector, ref worldViewProjection, out Vector3SByte v);

            result = new Vector3SByte((sbyte)(((1 + v.X) * 0.5 * width) + x), (sbyte)(((1 - v.Y) * 0.5 * height) + y), (sbyte)((v.Z * (maxZ - minZ)) + minZ));
        }

        /// <summary>
        /// Projects a 3D vector from object space into screen space. 
        /// </summary>
        /// <param name="vector">The vector to project.</param>
        /// <param name="x">The X position of the viewport.</param>
        /// <param name="y">The Y position of the viewport.</param>
        /// <param name="width">The width of the viewport.</param>
        /// <param name="height">The height of the viewport.</param>
        /// <param name="minZ">The minimum depth of the viewport.</param>
        /// <param name="maxZ">The maximum depth of the viewport.</param>
        /// <param name="worldViewProjection">The combined world-view-projection matrix.</param>
        /// <returns>The vector in screen space.</returns>
        public static Vector3SByte Project(Vector3SByte vector, sbyte x, sbyte y, sbyte width, sbyte height, sbyte minZ, sbyte maxZ, Matrix worldViewProjection)
        {
            Project(ref vector, x, y, width, height, minZ, maxZ, ref worldViewProjection, out Vector3SByte result);
            return result;
        }

        /// <summary>
        /// Projects a 3D vector from screen space into object space. 
        /// </summary>
        /// <param name="vector">The vector to project.</param>
        /// <param name="x">The X position of the viewport.</param>
        /// <param name="y">The Y position of the viewport.</param>
        /// <param name="width">The width of the viewport.</param>
        /// <param name="height">The height of the viewport.</param>
        /// <param name="minZ">The minimum depth of the viewport.</param>
        /// <param name="maxZ">The maximum depth of the viewport.</param>
        /// <param name="worldViewProjection">The combined world-view-projection matrix.</param>
        /// <param name="result">When the method completes, contains the vector in object space.</param>
        public static void Unproject(ref Vector3SByte vector, sbyte x, sbyte y, sbyte width, sbyte height, sbyte minZ, sbyte maxZ, ref Matrix worldViewProjection, out Vector3SByte result)
        {
            Vector3SByte v = new();
            Matrix.Invert(ref worldViewProjection, out Matrix matrix);

            v.X = (sbyte)((((vector.X - x) / width) * 2) - 1);
            v.Y = (sbyte)(-((((vector.Y - y) / height) * 2) - 1));
            v.Z = (sbyte)((vector.Z - minZ) / (maxZ - minZ));

            TransformCoordinate(ref v, ref matrix, out result);
        }

        /// <summary>
        /// Projects a 3D vector from screen space into object space. 
        /// </summary>
        /// <param name="vector">The vector to project.</param>
        /// <param name="x">The X position of the viewport.</param>
        /// <param name="y">The Y position of the viewport.</param>
        /// <param name="width">The width of the viewport.</param>
        /// <param name="height">The height of the viewport.</param>
        /// <param name="minZ">The minimum depth of the viewport.</param>
        /// <param name="maxZ">The maximum depth of the viewport.</param>
        /// <param name="worldViewProjection">The combined world-view-projection matrix.</param>
        /// <returns>The vector in object space.</returns>
        public static Vector3SByte Unproject(Vector3SByte vector, sbyte x, sbyte y, sbyte width, sbyte height, sbyte minZ, sbyte maxZ, Matrix worldViewProjection)
        {
            Unproject(ref vector, x, y, width, height, minZ, maxZ, ref worldViewProjection, out Vector3SByte result);
            return result;
        }

        /// <summary>
        /// Returns the reflection of a vector off a surface that has the specified normal. 
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="normal">Normal of the surface.</param>
        /// <param name="result">When the method completes, contains the reflected vector.</param>
        /// <remarks>Reflect only gives the direction of a reflection off a surface, it does not determine 
        /// whether the original vector was close enough to the surface to hit it.</remarks>
        public static void Reflect(ref Vector3SByte vector, ref Vector3SByte normal, out Vector3SByte result)
        {
            sbyte dot = (sbyte)((vector.X * normal.X) + (vector.Y * normal.Y) + (vector.Z * normal.Z));

            result.X = (sbyte)(vector.X - ((2 * dot) * normal.X));
            result.Y = (sbyte)(vector.Y - ((2 * dot) * normal.Y));
            result.Z = (sbyte)(vector.Z - ((2 * dot) * normal.Z));
        }

        /// <summary>
        /// Returns the reflection of a vector off a surface that has the specified normal. 
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="normal">Normal of the surface.</param>
        /// <returns>The reflected vector.</returns>
        /// <remarks>Reflect only gives the direction of a reflection off a surface, it does not determine 
        /// whether the original vector was close enough to the surface to hit it.</remarks>
        public static Vector3SByte Reflect(Vector3SByte vector, Vector3SByte normal)
        {
            Reflect(ref vector, ref normal, out Vector3SByte result);
            return result;
        }

        /// <summary>
        /// Orthogonalizes a list of vectors.
        /// </summary>
        /// <param name="destination">The list of orthogonalized vectors.</param>
        /// <param name="source">The list of vectors to orthogonalize.</param>
        /// <remarks>
        /// <para>Orthogonalization is the process of making all vectors orthogonal to each other. This
        /// means that any given vector in the list will be orthogonal to any other given vector in the
        /// list.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting vectors
        /// tend to be numerically unstable. The numeric stability decreases according to the vectors
        /// position in the list so that the first vector is the most stable and the last vector is the
        /// least stable.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        public static void Orthogonalize(Vector3SByte[] destination, params Vector3SByte[] source)
        {
            //Uses the modified Gram-Schmidt process.
            //q1 = m1
            //q2 = m2 - ((q1 ⋅ m2) / (q1 ⋅ q1)) * q1
            //q3 = m3 - ((q1 ⋅ m3) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m3) / (q2 ⋅ q2)) * q2
            //q4 = m4 - ((q1 ⋅ m4) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m4) / (q2 ⋅ q2)) * q2 - ((q3 ⋅ m4) / (q3 ⋅ q3)) * q3
            //q5 = ...

            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (destination.Length < source.Length)
                throw new ArgumentOutOfRangeException(nameof(destination), "The destination array must be of same length or larger length than the source array.");

            for (int i = 0; i < source.Length; ++i)
            {
                Vector3SByte newvector = source[i];

                for (int r = 0; r < i; ++r)
                {
                    newvector -= (sbyte)(Dot(destination[r], newvector) / Dot(destination[r], destination[r])) * destination[r];
                }

                destination[i] = newvector;
            }
        }

        /// <summary>
        /// Orthonormalizes a list of vectors.
        /// </summary>
        /// <param name="destination">The list of orthonormalized vectors.</param>
        /// <param name="source">The list of vectors to orthonormalize.</param>
        /// <remarks>
        /// <para>Orthonormalization is the process of making all vectors orthogonal to each
        /// other and making all vectors of unit length. This means that any given vector will
        /// be orthogonal to any other given vector in the list.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting vectors
        /// tend to be numerically unstable. The numeric stability decreases according to the vectors
        /// position in the list so that the first vector is the most stable and the last vector is the
        /// least stable.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        public static void Orthonormalize(Vector3SByte[] destination, params Vector3SByte[] source)
        {
            //Uses the modified Gram-Schmidt process.
            //Because we are making unit vectors, we can optimize the math for orthogonalization
            //and simplify the projection operation to remove the division.
            //q1 = m1 / |m1|
            //q2 = (m2 - (q1 ⋅ m2) * q1) / |m2 - (q1 ⋅ m2) * q1|
            //q3 = (m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2) / |m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2|
            //q4 = (m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3) / |m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3|
            //q5 = ...

            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (destination.Length < source.Length)
                throw new ArgumentOutOfRangeException(nameof(destination), "The destination array must be of same length or larger length than the source array.");

            for (int i = 0; i < source.Length; ++i)
            {
                Vector3SByte newvector = source[i];

                for (int r = 0; r < i; ++r)
                {
                    newvector -= (sbyte)Dot(destination[r], newvector) * destination[r];
                }

                newvector.Normalize();
                destination[i] = newvector;
            }
        }

        /// <summary>
        /// Transforms a 3D vector by the given <see cref="Quaternion"/> rotation.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> rotation to apply.</param>
        /// <param name="result">When the method completes, contains the transformed <see cref="Vector4"/>.</param>
        public static void Transform(ref Vector3SByte vector, ref Quaternion rotation, out Vector3SByte result)
        {
            float x = rotation.X + rotation.X;
            float y = rotation.Y + rotation.Y;
            float z = rotation.Z + rotation.Z;
            float wx = rotation.W * x;
            float wy = rotation.W * y;
            float wz = rotation.W * z;
            float xx = rotation.X * x;
            float xy = rotation.X * y;
            float xz = rotation.X * z;
            float yy = rotation.Y * y;
            float yz = rotation.Y * z;
            float zz = rotation.Z * z;

            result = new Vector3SByte(
                (sbyte)(((vector.X * ((1 - yy) - zz)) + (vector.Y * (xy - wz))) + (vector.Z * (xz + wy))),
                (sbyte)(((vector.X * (xy + wz)) + (vector.Y * ((1 - xx) - zz))) + (vector.Z * (yz - wx))),
                (sbyte)(((vector.X * (xz - wy)) + (vector.Y * (yz + wx))) + (vector.Z * ((1 - xx) - yy))));
        }

        /// <summary>
        /// Transforms a 3D vector by the given <see cref="Quaternion"/> rotation.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> rotation to apply.</param>
        /// <returns>The transformed <see cref="Vector4"/>.</returns>
        public static Vector3SByte Transform(Vector3SByte vector, Quaternion rotation)
        {
            Transform(ref vector, ref rotation, out Vector3SByte result);
            return result;
        }

        /// <summary>
        /// Transforms an array of vectors by the given <see cref="Quaternion"/> rotation.
        /// </summary>
        /// <param name="source">The array of vectors to transform.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> rotation to apply.</param>
        /// <param name="destination">The array for which the transformed vectors are stored.
        /// This array may be the same array as <paramref name="source"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        public static void Transform(Vector3SByte[] source, ref Quaternion rotation, Vector3SByte[] destination)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (destination.Length < source.Length)
                throw new ArgumentOutOfRangeException(nameof(destination), "The destination array must be of same length or larger length than the source array.");

            float x = rotation.X + rotation.X;
            float y = rotation.Y + rotation.Y;
            float z = rotation.Z + rotation.Z;
            float wx = rotation.W * x;
            float wy = rotation.W * y;
            float wz = rotation.W * z;
            float xx = rotation.X * x;
            float xy = rotation.X * y;
            float xz = rotation.X * z;
            float yy = rotation.Y * y;
            float yz = rotation.Y * z;
            float zz = rotation.Z * z;
            
            float num1 = ((1 - yy) - zz);
            float num2 = (xy - wz);
            float num3 = (xz + wy);
            float num4 = (xy + wz);
            float num5 = ((1 - xx) - zz);
            float num6 = (yz - wx);
            float num7 = (xz - wy);
            float num8 = (yz + wx);
            float num9 = ((1 - xx) - yy);

            for (int i = 0; i < source.Length; ++i)
            {
                destination[i] = new Vector3SByte(
                    (sbyte)(((source[i].X * num1) + (source[i].Y * num2)) + (source[i].Z * num3)),
                    (sbyte)(((source[i].X * num4) + (source[i].Y * num5)) + (source[i].Z * num6)),
                    (sbyte)(((source[i].X * num7) + (source[i].Y * num8)) + (source[i].Z * num9)));
            }
        }

        /// <summary>
        /// Transforms a 3D vector by the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="result">When the method completes, contains the transformed <see cref="Vector4"/>.</param>
        public static void Transform(ref Vector3SByte vector, ref Matrix transform, out Vector4 result)
        {
            result = new Vector4(
                (vector.X * transform.M11) + (vector.Y * transform.M21) + (vector.Z * transform.M31) + transform.M41,
                (vector.X * transform.M12) + (vector.Y * transform.M22) + (vector.Z * transform.M32) + transform.M42,
                (vector.X * transform.M13) + (vector.Y * transform.M23) + (vector.Z * transform.M33) + transform.M43,
                (vector.X * transform.M14) + (vector.Y * transform.M24) + (vector.Z * transform.M34) + transform.M44);
        }

        /// <summary>
        /// Transforms a 3D vector by the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="result">When the method completes, contains the transformed <see cref="Vector3SByte"/>.</param>
        public static void Transform(ref Vector3SByte vector, ref Matrix transform, out Vector3SByte result)
        {
            result = new Vector3SByte(
                (sbyte)((vector.X * transform.M11) + (vector.Y * transform.M21) + (vector.Z * transform.M31) + transform.M41),
                (sbyte)((vector.X * transform.M12) + (vector.Y * transform.M22) + (vector.Z * transform.M32) + transform.M42),
                (sbyte)((vector.X * transform.M13) + (vector.Y * transform.M23) + (vector.Z * transform.M33) + transform.M43));
        }

        /// <summary>
        /// Transforms a 3D vector by the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <returns>The transformed <see cref="Vector4"/>.</returns>
        public static Vector4 Transform(Vector3SByte vector, Matrix transform)
        {
            Transform(ref vector, ref transform, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Transforms an array of 3D vectors by the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="source">The array of vectors to transform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destination">The array for which the transformed vectors are stored.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        public static void Transform(Vector3SByte[] source, ref Matrix transform, Vector4[] destination)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (destination.Length < source.Length)
                throw new ArgumentOutOfRangeException(nameof(destination), "The destination array must be of same length or larger length than the source array.");

            for (int i = 0; i < source.Length; ++i)
            {
                Transform(ref source[i], ref transform, out destination[i]);
            }
        }

        /// <summary>
        /// Performs a coordinate transformation using the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="coordinate">The coordinate vector to transform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="result">When the method completes, contains the transformed coordinates.</param>
        /// <remarks>
        /// A coordinate transform performs the transformation with the assumption that the w component
        /// is one. The four dimensional vector obtained from the transformation operation has each
        /// component in the vector divided by the w component. This forces the wcomponent to be one and
        /// therefore makes the vector homogeneous. The homogeneous vector is often prefered when working
        /// with coordinates as the w component can safely be ignored.
        /// </remarks>
        public static void TransformCoordinate(ref Vector3SByte coordinate, ref Matrix transform, out Vector3SByte result)
        {
            var invW = 1f / ((coordinate.X * transform.M14) + (coordinate.Y * transform.M24) + (coordinate.Z * transform.M34) + transform.M44);
            result = new Vector3SByte(
                (sbyte)(((coordinate.X * transform.M11) + (coordinate.Y * transform.M21) + (coordinate.Z * transform.M31) + transform.M41) * invW),
                (sbyte)(((coordinate.X * transform.M12) + (coordinate.Y * transform.M22) + (coordinate.Z * transform.M32) + transform.M42) * invW),
                (sbyte)(((coordinate.X * transform.M13) + (coordinate.Y * transform.M23) + (coordinate.Z * transform.M33) + transform.M43) * invW));
        }

        /// <summary>
        /// Performs a coordinate transformation using the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="coordinate">The coordinate vector to transform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <returns>The transformed coordinates.</returns>
        /// <remarks>
        /// A coordinate transform performs the transformation with the assumption that the w component
        /// is one. The four dimensional vector obtained from the transformation operation has each
        /// component in the vector divided by the w component. This forces the wcomponent to be one and
        /// therefore makes the vector homogeneous. The homogeneous vector is often prefered when working
        /// with coordinates as the w component can safely be ignored.
        /// </remarks>
        public static Vector3SByte TransformCoordinate(Vector3SByte coordinate, Matrix transform)
        {
            TransformCoordinate(ref coordinate, ref transform, out Vector3SByte result);
            return result;
        }

        /// <summary>
        /// Performs a coordinate transformation on an array of vectors using the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="source">The array of coordinate vectors to trasnform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destination">The array for which the transformed vectors are stored.
        /// This array may be the same array as <paramref name="source"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        /// <remarks>
        /// A coordinate transform performs the transformation with the assumption that the w component
        /// is one. The four dimensional vector obtained from the transformation operation has each
        /// component in the vector divided by the w component. This forces the wcomponent to be one and
        /// therefore makes the vector homogeneous. The homogeneous vector is often prefered when working
        /// with coordinates as the w component can safely be ignored.
        /// </remarks>
        public static void TransformCoordinate(Vector3SByte[] source, ref Matrix transform, Vector3SByte[] destination)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (destination.Length < source.Length)
                throw new ArgumentOutOfRangeException(nameof(destination), "The destination array must be of same length or larger length than the source array.");

            for (int i = 0; i < source.Length; ++i)
            {
                TransformCoordinate(ref source[i], ref transform, out destination[i]);
            }
        }

        /// <summary>
        /// Performs a normal transformation using the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="normal">The normal vector to transform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="result">When the method completes, contains the transformed normal.</param>
        /// <remarks>
        /// A normal transform performs the transformation with the assumption that the w component
        /// is zero. This causes the fourth row and fourth collumn of the matrix to be unused. The
        /// end result is a vector that is not translated, but all other transformation properties
        /// apply. This is often prefered for normal vectors as normals purely represent direction
        /// rather than location because normal vectors should not be translated.
        /// </remarks>
        public static void TransformNormal(ref Vector3SByte normal, ref Matrix transform, out Vector3SByte result)
        {
            result = new Vector3SByte(
                (sbyte)((normal.X * transform.M11) + (normal.Y * transform.M21) + (normal.Z * transform.M31)),
                (sbyte)((normal.X * transform.M12) + (normal.Y * transform.M22) + (normal.Z * transform.M32)),
                (sbyte)((normal.X * transform.M13) + (normal.Y * transform.M23) + (normal.Z * transform.M33)));
        }

        /// <summary>
        /// Performs a normal transformation using the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="normal">The normal vector to transform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <returns>The transformed normal.</returns>
        /// <remarks>
        /// A normal transform performs the transformation with the assumption that the w component
        /// is zero. This causes the fourth row and fourth collumn of the matrix to be unused. The
        /// end result is a vector that is not translated, but all other transformation properties
        /// apply. This is often prefered for normal vectors as normals purely represent direction
        /// rather than location because normal vectors should not be translated.
        /// </remarks>
        public static Vector3SByte TransformNormal(Vector3SByte normal, Matrix transform)
        {
            TransformNormal(ref normal, ref transform, out Vector3SByte result);
            return result;
        }

        /// <summary>
        /// Performs a normal transformation on an array of vectors using the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="source">The array of normal vectors to transform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destination">The array for which the transformed vectors are stored.
        /// This array may be the same array as <paramref name="source"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        /// <remarks>
        /// A normal transform performs the transformation with the assumption that the w component
        /// is zero. This causes the fourth row and fourth collumn of the matrix to be unused. The
        /// end result is a vector that is not translated, but all other transformation properties
        /// apply. This is often prefered for normal vectors as normals purely represent direction
        /// rather than location because normal vectors should not be translated.
        /// </remarks>
        public static void TransformNormal(Vector3SByte[] source, ref Matrix transform, Vector3SByte[] destination)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (destination.Length < source.Length)
                throw new ArgumentOutOfRangeException(nameof(destination), "The destination array must be of same length or larger length than the source array.");

            for (int i = 0; i < source.Length; ++i)
            {
                TransformNormal(ref source[i], ref transform, out destination[i]);
            }
        }

        /// <summary>
        /// Calculate the yaw/pitch/roll rotation equivalent to the provided quaterion.
        /// </summary>
        /// <param name="quaternion">The input rotation as quaternion</param>
        /// <returns>The equivation yaw/pitch/roll rotation</returns>
        public static Vector3SByte RotationYawPitchRoll(Quaternion quaternion)
        {
            Quaternion.RotationYawPitchRoll(ref quaternion, out float rX, out float rY, out float rZ);
            return new Vector3SByte((sbyte)rX, (sbyte)rY, (sbyte)rZ);
        }

        /// <summary>
        /// Calculate the yaw/pitch/roll rotation equivalent to the provided quaterion. 
        /// </summary>
        /// <param name="quaternion">The input rotation as quaternion</param>
        /// <param name="yawPitchRoll">The equivation yaw/pitch/roll rotation</param>
        public static void RotationYawPitchRoll(ref Quaternion quaternion, ref Vector3SByte yawPitchRoll)
        {
            float rX = yawPitchRoll.X;
            float rY = yawPitchRoll.Y;
            float rZ = yawPitchRoll.Z;
            Quaternion.RotationYawPitchRoll(ref quaternion, out rX, out rY, out rZ);
            yawPitchRoll.X = (sbyte)rX;
            yawPitchRoll.Y = (sbyte)rY;
            yawPitchRoll.Z = (sbyte)rZ;
        }


        /// <summary>
        /// Rotates the source around the target by the rotation angle around the supplied axis. 
        /// </summary>
        /// <param name="source">The position to rotate.</param>
        /// <param name="target">The point to rotate around.</param>
        /// <param name="axis">The axis of rotation.</param>
        /// <param name="angle">The angle to rotate by in radians.</param>
        /// <returns>The rotated vector.</returns>
        public static Vector3SByte RotateAround(in Vector3SByte source, in Vector3SByte target, in Vector3SByte axis, sbyte angle)
        {
            Vector3SByte local = source - target;
            Quaternion q = Quaternion.RotationAxis(new Vector3(axis.X, axis.Y, axis.Z), angle);
            Vector3 newLocal = new(local.X, local.Y, local.Z);
            q.Rotate(ref newLocal);
            return target + local;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="left">The first vector to add.</param>
        /// <param name="right">The second vector to add.</param>
        /// <returns>The sum of the two vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte operator +(Vector3SByte left, Vector3SByte right)
        {
            return new Vector3SByte((sbyte)(left.X + right.X), (sbyte)(left.Y + right.Y), (sbyte)(left.Z + right.Z));
        }

        /// <summary>
        /// Assert a vector (return it unchanged).
        /// </summary>
        /// <param name="value">The vector to assert (unchange).</param>
        /// <returns>The asserted (unchanged) vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte operator +(Vector3SByte value)
        {
            return value;
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="left">The first vector to subtract.</param>
        /// <param name="right">The second vector to subtract.</param>
        /// <returns>The difference of the two vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte operator -(Vector3SByte left, Vector3SByte right)
        {
            return new Vector3SByte((sbyte)(left.X - right.X), (sbyte)(left.Y - right.Y), (sbyte)(left.Z - right.Z));
        }

        /// <summary>
        /// Reverses the direction of a given vector.
        /// </summary>
        /// <param name="value">The vector to negate.</param>
        /// <returns>A vector facing in the opposite direction.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte operator -(Vector3SByte value)
        {
            return new Vector3SByte((sbyte)(-value.X), (sbyte)(-value.Y), (sbyte)(-value.Z));
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scale">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte operator *(sbyte scale, Vector3SByte value)
        {
            return new Vector3SByte((sbyte)(value.X * scale), (sbyte)(value.Y * scale), (sbyte)(value.Z * scale));
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scale">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte operator *(Vector3SByte value, sbyte scale)
        {
            return new Vector3SByte((sbyte)(value.X * scale), (sbyte)(value.Y * scale), (sbyte)(value.Z * scale));
        }

        /// <summary>
        /// Modulates a vector with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">The first vector to multiply.</param>
        /// <param name="right">The second vector to multiply.</param>
        /// <returns>The multiplication of the two vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte operator *(Vector3SByte left, Vector3SByte right)
        {
            return new Vector3SByte((sbyte)(left.X * right.X), (sbyte)(left.Y * right.Y), (sbyte)(left.Z * right.Z));
        }

        /// <summary>
        /// Adds a vector with the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scale">The amount by which to scale the vector.</param>
        /// <returns>The vector offset.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte operator +(Vector3SByte value, sbyte scale)
        {
            return new Vector3SByte((sbyte)(value.X + scale), (sbyte)(value.Y + scale), (sbyte)(value.Z + scale));
        }

        /// <summary>
        /// Substracts a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scale">The amount by which to scale the vector.</param>
        /// <returns>The vector offset.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte operator -(Vector3SByte value, sbyte scale)
        {
            return new Vector3SByte((sbyte)(value.X - scale), (sbyte)(value.Y - scale), (sbyte)(value.Z - scale));
        }

        /// <summary>
        /// Divides a numerator by a vector.
        /// </summary>
        /// <param name="numerator">The numerator.</param>
        /// <param name="value">The value.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte operator /(sbyte numerator, Vector3SByte value)
        {
            return new Vector3SByte((sbyte)(numerator / value.X), (sbyte)(numerator / value.Y), (sbyte)(numerator / value.Z));
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="scale">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte operator /(Vector3SByte value, sbyte scale)
        {
            return new Vector3SByte((sbyte)(value.X / scale), (sbyte)(value.Y / scale), (sbyte)(value.Z / scale));
        }

        /// <summary>
        /// Divides a vector by the given vector, component-wise.
        /// </summary>
        /// <param name="value">The vector to scale.</param>
        /// <param name="by">The by.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3SByte operator /(Vector3SByte value, Vector3SByte by)
        {
            return new Vector3SByte((sbyte)(value.X / by.X), (sbyte)(value.Y / by.Y), (sbyte)(value.Z / by.Z));
        }

        /// <summary>
        /// Tests for equality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Vector3SByte left, Vector3SByte right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests for inequality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Vector3SByte left, Vector3SByte right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Vector3SByte"/> to <see cref="Vector2"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Vector2(Vector3SByte value)
        {
            return new Vector2(value.X, value.Y);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Vector3SByte"/> to <see cref="Vector4"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Vector4(Vector3SByte value)
        {
            return new Vector4(new Vector3(value.X, value.Y, value.Z), 0);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Vector3SByte"/> to <see cref="Int3"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Int3(Vector3SByte value)
        {
            return new Int3(value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Tests whether one 3D vector is near another 3D vector.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <param name="epsilon">The epsilon.</param>
        /// <returns><c>true</c> if left and right are near another 3D, <c>false</c> otherwise</returns>
        public static bool NearEqual(Vector3SByte left, Vector3SByte right, Vector3SByte epsilon)
        {
            return NearEqual(ref left, ref right, ref epsilon);
        }

        /// <summary>
        /// Tests whether one 3D vector is near another 3D vector.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <param name="epsilon">The epsilon.</param>
        /// <returns><c>true</c> if left and right are near another 3D, <c>false</c> otherwise</returns>
        public static bool NearEqual(ref Vector3SByte left, ref Vector3SByte right, ref Vector3SByte epsilon)
        {
            return MathUtil.WithinEpsilon(left.X, right.X, epsilon.X) &&
                    MathUtil.WithinEpsilon(left.Y, right.Y, epsilon.Y) &&
                    MathUtil.WithinEpsilon(left.Z, right.Z, epsilon.Z);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override readonly string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2}", X, Y, Z);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public readonly string ToString(string format)
        {
            if (format == null)
                return ToString();

            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2}", X.ToString(format, CultureInfo.CurrentCulture),
                Y.ToString(format, CultureInfo.CurrentCulture), Z.ToString(format, CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public readonly string ToString(IFormatProvider? formatProvider)
        {
            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2}", X, Y, Z);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (format == null)
                return ToString(formatProvider);

            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2}", X.ToString(format, formatProvider),
                Y.ToString(format, formatProvider), Z.ToString(format, formatProvider));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override readonly int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="Vector3SByte"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Vector3SByte"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Vector3SByte"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public readonly bool Equals(Vector3SByte other)
        {
            return (MathF.Abs(other.X - X) < MathUtil.ZeroTolerance &&
                MathF.Abs(other.Y - Y) < MathUtil.ZeroTolerance &&
                MathF.Abs(other.Z - Z) < MathUtil.ZeroTolerance);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override readonly bool Equals(object? value)
        {
            if (value == null)
                return false;

            if (value.GetType() != GetType())
                return false;

            return Equals((Vector3SByte)value);
        }

        /// <summary>
        /// Deconstructs the vector's components into named variables.
        /// </summary>
        /// <param name="x">The X component</param>
        /// <param name="y">The Y component</param>
        /// <param name="z">The Z component</param>
        public readonly void Deconstruct(out sbyte x, out sbyte y, out sbyte z)
        {
            x = X;
            y = Y;
            z = Z;
        }

#if WPFInterop
        /// <summary>
        /// Performs an implicit conversion from <see cref="Vector3SByte"/> to <see cref="System.Windows.Media.Media3D.Vector3SByteD"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator System.Windows.Media.Media3D.Vector3SByteD(Vector3SByte value)
        {
            return new System.Windows.Media.Media3D.Vector3SByteD(value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="System.Windows.Media.Media3D.Vector3SByteD"/> to <see cref="Vector3SByte"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Vector3SByte(System.Windows.Media.Media3D.Vector3SByteD value)
        {
            return new Vector3SByte((sbyte)value.X, (sbyte)value.Y, (sbyte)value.Z);
        }
#endif

#if XnaInterop
        /// <summary>
        /// Performs an implicit conversion from <see cref="Vector3SByte"/> to <see cref="Microsoft.Xna.Framework.Vector3SByte"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Microsoft.Xna.Framework.Vector3SByte(Vector3SByte value)
        {
            return new Microsoft.Xna.Framework.Vector3SByte(value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Microsoft.Xna.Framework.Vector3SByte"/> to <see cref="Vector3SByte"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Vector3SByte(Microsoft.Xna.Framework.Vector3SByte value)
        {
            return new Vector3SByte(value.X, value.Y, value.Z);
        }
#endif
    }
}

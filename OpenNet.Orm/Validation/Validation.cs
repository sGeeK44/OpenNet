// -------------------------------------------------------------------------------------------------------
// LICENSE INFORMATION
//
// - This software is licensed under the MIT shared source license.
// - The "official" source code for this project is maintained at http://oncfext.codeplex.com
//
// Copyright (c) 2010 OpenNETCF Consulting
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
// associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial 
// portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// -------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
// ReSharper disable UseStringInterpolation

// ReSharper disable ConvertPropertyToExpressionBody

namespace OpenNet.Orm.Validation
{
    public sealed class Validation
    {
        private readonly List<Exception> _exceptions;

        public IEnumerable<Exception> Exceptions { get { return _exceptions; } }

        public Validation()
        {
            _exceptions = new List<Exception>(1); // optimize for only having 1 exception
        }

        public const string UnspecifiedParameterName = "{unspecified}";

        private void AddExceptionInternal(Exception exception)
        {
            AddException(exception);
        }

        public void AddException(Exception ex)
        {
            lock (_exceptions)
            {
                _exceptions.Add(ex);
            }
        }

        public Validation Check()
        {
            if (Exceptions.Count() != 0)
                throw new ValidationException(this);
            return this;
        }

        public Validation HasValue<T>(T? t)
            where T : struct
        {
            return HasValue(t, null);
        }

        public Validation HasValue<T>(T? t, string paramName)
            where T : struct
        {
            if (!t.HasValue)
            {
                AddExceptionInternal(new ArgumentException(paramName ?? UnspecifiedParameterName));
            }

            return this;
        }

        public Validation HasNoValue<T>(T? t)
            where T : struct
        {
            return HasNoValue(t, null);
        }

        public Validation HasNoValue<T>(T? t, string paramName)
            where T : struct
        {
            if (t.HasValue)
            {
                AddExceptionInternal(new ArgumentException(paramName ?? UnspecifiedParameterName));
            }

            return this;
        }

        public Validation IsNull<T>(T t)
            where T : class
        {
            return IsNull(t, null);
        }

        public Validation IsNull<T>(T t, string paramName)
            where T : class
        {
            if (t != null)
            {
                AddExceptionInternal(new ArgumentException(paramName ?? UnspecifiedParameterName));
            }

            return this;
        }

        public Validation IsNotNull<T>(T t)
            where T : class
        {
            return IsNotNull(t, null);
        }

        public Validation IsNotNull<T>(T theObject, string paramName)
            where T : class
        {
            if (theObject == null)
            {
                AddExceptionInternal(new ArgumentNullException(paramName ?? UnspecifiedParameterName));
            }
            return this;
        }

        public Validation IsNotNull<T>(T? t)
            where T : struct
        {
            return IsNotNull(t, null);
        }

        public Validation IsNotNull<T>(T? theObject, string paramName)
            where T : struct
        {
            if (theObject == null)
            {
                AddExceptionInternal(new ArgumentNullException(paramName ?? UnspecifiedParameterName));
            }

            return this;
        }


        public Validation IsTrue(bool condition)
        {
            if (!condition)
            {
                AddExceptionInternal(new ArgumentException());
            }
            return this;
        }

        public Validation IsFalse(bool condition)
        {
            if (condition)
            {
                AddExceptionInternal(new ArgumentException());
            }
            return this;
        }

        public Validation IsNotNullOrEmpty(string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                AddExceptionInternal(new ArgumentNullException());
            }
            return this;
        }

        public Validation IsNotNullOrEmpty(string item, string paramName)
        {
            if (string.IsNullOrEmpty(item))
            {
                AddExceptionInternal(new ArgumentNullException());
            }
            return this;
        }

        public Validation IsPositive(long value)
        {
            return IsPositive(value, null);
        }

        public Validation IsPositive(long value, string paramName)
        {
            if (value <= 0)
            {
                AddExceptionInternal(new ArgumentOutOfRangeException(paramName ?? UnspecifiedParameterName, "must be positive, but was " + value));
            }
            return this;
        }

        public Validation IsPositive(decimal value)
        {
            return IsPositive(value, null);
        }

        public Validation IsPositive(decimal value, string paramName)
        {
            if (value <= 0)
            {
                AddExceptionInternal(new ArgumentOutOfRangeException(paramName ?? UnspecifiedParameterName, "must be positive, but was " + value));
            }
            return this;
        }

        public Validation IsPositive(double value)
        {
            return IsPositive(value, null);
        }

        public Validation IsPositive(double value, string paramName)
        {
            if (value <= 0)
            {
                AddExceptionInternal(new ArgumentOutOfRangeException(paramName ?? UnspecifiedParameterName, "must be positive, but was " + value));
            }
            return this;
        }

        public Validation IsPositiveOrZero(long value)
        {
            return IsPositiveOrZero(value, null);
        }

        public Validation IsPositiveOrZero(long value, string paramName)
        {
            if (value < 0)
            {
                AddExceptionInternal(new ArgumentOutOfRangeException(paramName, "must be >= 0, but was " + value));
            }
            return this;
        }

        public Validation IsLessThanOrEqualTo(long value, long upperLimit)
        {
            return IsLessThanOrEqualTo(value, upperLimit, null);
        }

        public Validation IsLessThanOrEqualTo(long value, long upperLimit, string paramName)
        {
            if (value > upperLimit)
            {
                AddExceptionInternal(new ArgumentOutOfRangeException(string.Format("{0} must be <= {1}, but was {2}", paramName, upperLimit, value)));
            }
            return this;
        }

        public Validation IsLessThan(long value, long upperLimit)
        {
            return IsLessThan(value, upperLimit, null);
        }

        public Validation IsLessThan(long value, long upperLimit, string paramName)
        {
            if (value >= upperLimit)
            {
                AddExceptionInternal(new ArgumentOutOfRangeException(string.Format("{0} must be < {1}, but was {2}", paramName, upperLimit, value)));
            }
            return this;
        }

        public Validation IsGreaterThanOrEqualTo(long value, long lowerLimit)
        {
            return IsGreaterThanOrEqualTo(value, lowerLimit, null);
        }

        public Validation IsGreaterThanOrEqualTo(long value, long lowerLimit, string paramName)
        {
            if (value < lowerLimit)
            {
                AddExceptionInternal(new ArgumentOutOfRangeException(string.Format("{0} must be >= {1}, but was {2}", paramName, lowerLimit, value)));
            }
            return this;
        }

        public Validation IsGreaterThan(long value, long lowerLimit)
        {
            return IsGreaterThan(value, lowerLimit, null);
        }

        public Validation IsGreaterThan(long value, long lowerLimit, string paramName)
        {
            if (value <= lowerLimit)
            {
                AddExceptionInternal(new ArgumentOutOfRangeException(string.Format("{0} must be > {1}, but was {2}", paramName, lowerLimit, value)));
            }
            return this;
        }

        public Validation IsWithinBoundsInclusive(long value, long lowerLimit, long upperLimit)
        {
            return IsWithinBoundsInclusive(value, lowerLimit, upperLimit, null);
        }

        public Validation IsWithinBoundsInclusive(long value, long lowerLimit, long upperLimit, string paramName)
        {
            if ((value < lowerLimit) || (value > upperLimit))
            {
                AddExceptionInternal(new ArgumentOutOfRangeException(string.Format("{0} must be < {1} and > {2}, but was {3}", paramName ?? UnspecifiedParameterName, upperLimit, lowerLimit, value)));
            }
            return this;
        }

        public Validation IsWithinBoundsInclusive(double value, double lowerLimit, double upperLimit)
        {
            return IsWithinBoundsInclusive(value, lowerLimit, upperLimit, null);
        }

        public Validation IsWithinBoundsInclusive(double value, double lowerLimit, double upperLimit, string paramName)
        {
            if ((value < lowerLimit) || (value > upperLimit))
            {
                AddExceptionInternal(new ArgumentOutOfRangeException(string.Format("{0} must be < {1} and > {2}, but was {3}", paramName ?? UnspecifiedParameterName, upperLimit, lowerLimit, value)));
            }
            return this;
        }

        public Validation IsPositiveIfNotNull(decimal? t)
        {
            return IsPositiveIfNotNull(t, null);
        }

        public Validation IsPositiveIfNotNull(decimal? t, string paramName)
        {
            if (t.HasValue)
            {
                return IsPositive((long)(t.Value), paramName);
            }

            return this;
        }

        public Validation AreNotEqual<T>(T actual, T expected)
        {
            return AreNotEqual(actual, expected, null);
        }

        public Validation AreNotEqual<T>(T actual, T expected, string paramName)
        {
            if (actual.Equals(expected))
            {
                AddExceptionInternal(new ArgumentOutOfRangeException(string.Format("parameter {0} is {1}, not the expected {2}", paramName ?? UnspecifiedParameterName, actual, expected)));
            }

            return this;
        }

        public Validation AreEqual<T>(T actual, T expected)
        {
            return AreEqual(actual, expected, null);
        }

        public Validation AreEqual<T>(T actual, T expected, string paramName)
        {
            if (!actual.Equals(expected))
            {
                AddExceptionInternal(new ArgumentOutOfRangeException(string.Format("parameter {0} is {1}, not the expected {2}", paramName ?? UnspecifiedParameterName, actual, expected)));
            }

            return this;
        }

        public Validation FileExists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                AddExceptionInternal(new FileNotFoundException(string.Format("File '{0}' not found", filePath)));
            }

            return this;
        }
    }
}

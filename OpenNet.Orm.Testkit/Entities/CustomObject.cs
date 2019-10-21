using System;
using System.Collections.Generic;
using System.Text;
using OpenNet.Orm.Entity.Fields;

namespace OpenNet.Orm.Testkit.Entities
{
    public class CustomObject : ICustomSqlField
    {
        public string ObjectName { get; set; }
        public Guid Identifier { get; set; }
        public int SomeIntProp { get; set; }

        private void InitValue(byte[] data)
        {
            // deserialization ctor
            var offset = 0;

            // get the name length
            var nameLength = BitConverter.ToInt32(data, offset);

            // get the name bytes
            offset += 4; // past the length
            ObjectName = Encoding.ASCII.GetString(data, offset, nameLength);

            // get the GUID
            offset += nameLength;
            var guidData = new byte[16];
            // we must copy the data since Guid doesn't have a ctor that allows us to specify an offset
            Buffer.BlockCopy(data, offset, guidData, 0, guidData.Length);
            Identifier = new Guid(guidData);

            // get the int property
            offset += guidData.Length;
            SomeIntProp = BitConverter.ToInt32(data, offset);
        }

        private byte[] AsByteArray()
        {
            var buffer = new List<byte>();

            var nameData = Encoding.ASCII.GetBytes(ObjectName);

            // store the name length
            buffer.AddRange(BitConverter.GetBytes(nameData.Length));

            // store the name data
            buffer.AddRange(nameData);

            // store the GUID
            buffer.AddRange(Identifier.ToByteArray());

            // store the IntProp
            buffer.AddRange(BitConverter.GetBytes(SomeIntProp));

            return buffer.ToArray();
        }

        public byte Precision { get; set;  }

        public byte Scale { get; set; }

        public object ToSqlValue()
        {
            return AsByteArray();
        }

        public void FromSqlValue(object value)
        {
            InitValue(value as byte[]);
        }
    }
}

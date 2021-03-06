﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NetworkHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-net-types.html
    /// </remarks>
    [TypeMapping("macaddr", NpgsqlDbType.MacAddr, typeof(PhysicalAddress))]
    internal class MacaddrHandler : TypeHandler<PhysicalAddress>,
        ISimpleTypeReader<PhysicalAddress>, ISimpleTypeWriter,
        ISimpleTypeReader<string>
    {
        byte[] _bytes;

        public PhysicalAddress Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            Contract.Assume(len == 6);

            if (_bytes == null) {
                _bytes = new byte[6];
            }

            buf.ReadBytes(_bytes, 0, 6);
            return new PhysicalAddress(_bytes);
        }

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription).ToString();
        }

        public int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            var address = value as PhysicalAddress;
            if (address == null)
                throw CreateConversionException(value.GetType());
            if (address.GetAddressBytes().Length != 6)
                throw new FormatException("MAC addresses must have length 6 in PostgreSQL");
            return 6;
        }

        public void Write(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            buf.WriteBytes(((PhysicalAddress)value).GetAddressBytes(), 0, 6);
        }
    }
}

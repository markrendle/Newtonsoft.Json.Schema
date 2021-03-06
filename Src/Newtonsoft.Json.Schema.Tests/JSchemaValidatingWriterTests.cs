﻿#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public class JSchemaValidatingWriterTests : TestFixtureBase
    {
        [Test]
        public void ArrayBasicValidation_Pass()
        {
            JSchema schema = new JSchema();
            schema.Type = JSchemaType.Array;
            schema.Items.Add(new JSchema
            {
                Type = JSchemaType.Integer
            });

            SchemaValidationEventArgs a = null;

            StringWriter sw = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);
            JSchemaValidatingWriter validatingWriter = new JSchemaValidatingWriter(writer);
            validatingWriter.Schema = schema;
            validatingWriter.ValidationEventHandler += (sender, args) =>
            {
                a = args;
            };

            validatingWriter.WriteStartArray();
            validatingWriter.WriteValue(10);
            validatingWriter.WriteValue(10);
            validatingWriter.WriteEndArray();

            Assert.IsNull(a);
        }

        [Test]
        public void ArrayBasicValidation_Fail()
        {
            JSchema schema = new JSchema();
            schema.Type = JSchemaType.Array;
            schema.Items.Add(new JSchema
            {
                Type = JSchemaType.Integer
            });

            SchemaValidationEventArgs a = null;

            StringWriter sw = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);
            JSchemaValidatingWriter validatingWriter = new JSchemaValidatingWriter(writer);
            validatingWriter.Schema = schema;
            validatingWriter.ValidationEventHandler += (sender, args) =>
            {
                a = args;
            };

            validatingWriter.WriteStartArray();

            validatingWriter.WriteValue("string");
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got String.", a.Message);
            Assert.AreEqual("string", a.ValidationError.Value);
            a = null;

            validatingWriter.WriteValue(true);
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got Boolean.", a.Message);
            Assert.AreEqual(true, a.ValidationError.Value);
            a = null;

            validatingWriter.WriteEndArray();
            Assert.IsNull(a);
        }

        [Test]
        public void ObjectBasicValidation_Pass()
        {
            JSchema schema = new JSchema();
            schema.Type = JSchemaType.Object;
            schema.Properties.Add("prop1", new JSchema
            {
                Type = JSchemaType.Integer
            });
            schema.Properties.Add("prop2", new JSchema
            {
                Type = JSchemaType.Boolean
            });

            SchemaValidationEventArgs a = null;

            StringWriter sw = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);
            JSchemaValidatingWriter validatingWriter = new JSchemaValidatingWriter(writer);
            validatingWriter.Schema = schema;
            validatingWriter.ValidationEventHandler += (sender, args) =>
            {
                a = args;
            };

            validatingWriter.WriteStartObject();
            validatingWriter.WritePropertyName("prop1");
            validatingWriter.WriteValue(43);
            validatingWriter.WritePropertyName("prop2");
            validatingWriter.WriteValue(true);
            validatingWriter.WriteEndObject();

            Assert.IsNull(a);
        }

        [Test]
        public void ObjectBasicValidation_Fail()
        {
            JSchema schema = new JSchema();
            schema.Type = JSchemaType.Object;
            schema.Properties.Add("prop1", new JSchema
            {
                Type = JSchemaType.Integer
            });
            schema.Properties.Add("prop2", new JSchema
            {
                Type = JSchemaType.Boolean
            });

            SchemaValidationEventArgs a = null;

            StringWriter sw = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);
            JSchemaValidatingWriter validatingWriter = new JSchemaValidatingWriter(writer);
            validatingWriter.Schema = schema;
            validatingWriter.ValidationEventHandler += (sender, args) =>
            {
                a = args;
            };

            validatingWriter.WriteStartObject();

            validatingWriter.WritePropertyName("prop1");
            validatingWriter.WriteValue(true);
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got Boolean.", a.Message);
            Assert.AreEqual(true, a.ValidationError.Value);
            a = null;

            validatingWriter.WritePropertyName("prop2");
            validatingWriter.WriteValue(45);
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Boolean but got Integer.", a.Message);
            Assert.AreEqual(45, a.ValidationError.Value);
            a = null;

            validatingWriter.WritePropertyName("prop3");
            validatingWriter.WriteValue(45);

            validatingWriter.WriteEndObject();

            Assert.IsNull(a);
        }

        [Test]
        public void NestedScopes()
        {
            JSchema schema = JSchema.Parse(@"{
                ""properties"": {
                    ""foo"": {""type"": ""integer""},
                    ""bar"": {""type"": ""string""}
                }
            }");

            SchemaValidationEventArgs a = null;

            StringWriter sw = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);
            JSchemaValidatingWriter validatingWriter = new JSchemaValidatingWriter(writer);
            validatingWriter.Schema = schema;
            validatingWriter.ValidationEventHandler += (sender, args) =>
            {
                a = args;
            };

            validatingWriter.WriteStartObject();
            validatingWriter.WritePropertyName("foo");
            validatingWriter.WriteStartArray();

            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got Array.", a.Message);
            Assert.AreEqual(null, a.ValidationError.Value);
            a = null;

            validatingWriter.WriteEndArray();
            validatingWriter.WritePropertyName("bar");
            validatingWriter.WriteStartObject();

            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected String but got Object.", a.Message);
            Assert.AreEqual(null, a.ValidationError.Value);
            a = null;

            validatingWriter.WriteEndObject();
            validatingWriter.WriteEndObject();

            Assert.IsNull(a);
        }

        [Test]
        public void WriteValue()
        {
            JSchema schema = new JSchema();
            schema.Type = JSchemaType.Array;
            schema.Items.Add(new JSchema
            {
                Type = JSchemaType.Integer
            });

            SchemaValidationEventArgs a = null;

            StringWriter sw = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);
            writer.Formatting = Formatting.Indented;
            JSchemaValidatingWriter validatingWriter = new JSchemaValidatingWriter(writer);
            validatingWriter.Schema = schema;
            validatingWriter.ValidationEventHandler += (sender, args) =>
            {
                a = args;
            };

            validatingWriter.WriteStartArray();

            validatingWriter.WriteValue("string");
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got String.", a.Message);
            Assert.AreEqual("string", a.ValidationError.Value);
            a = null;

            validatingWriter.WriteValue((string)null);
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got Null.", a.Message);
            Assert.AreEqual(null, a.ValidationError.Value);
            a = null;

            validatingWriter.WriteValue('e');
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got String.", a.Message);
            Assert.AreEqual("e", a.ValidationError.Value);
            a = null;

            validatingWriter.WriteValue(true);
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got Boolean.", a.Message);
            Assert.AreEqual(true, a.ValidationError.Value);
            a = null;

            validatingWriter.WriteValue((int?)null);
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got Null.", a.Message);
            Assert.AreEqual(null, a.ValidationError.Value);
            a = null;

            validatingWriter.WriteNull();
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got Null.", a.Message);
            Assert.AreEqual(null, a.ValidationError.Value);
            a = null;

            validatingWriter.WriteUndefined();
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got Undefined.", a.Message);
            Assert.AreEqual(null, a.ValidationError.Value);
            a = null;

            validatingWriter.WriteValue((ushort)12);
            Assert.IsNull(a);
            validatingWriter.WriteValue((short)12);
            Assert.IsNull(a);
            validatingWriter.WriteValue((byte)12);
            Assert.IsNull(a);
            validatingWriter.WriteValue((sbyte)12);
            Assert.IsNull(a);
            validatingWriter.WriteValue((long)12);
            Assert.IsNull(a);
            validatingWriter.WriteValue((ulong)12);
            Assert.IsNull(a);
            validatingWriter.WriteValue((int)12);
            Assert.IsNull(a);
            validatingWriter.WriteValue((uint)12);
            Assert.IsNull(a);

            validatingWriter.WriteValue(1.1m);
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got Float.", a.Message);
            Assert.AreEqual(1.1d, a.ValidationError.Value);
            a = null;

            validatingWriter.WriteValue(1.1d);
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got Float.", a.Message);
            Assert.AreEqual(1.1d, a.ValidationError.Value);
            a = null;

            validatingWriter.WriteValue(1.1f);
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got Float.", a.Message);
            Assert.AreEqual(1.1f, a.ValidationError.Value);
            a = null;

            validatingWriter.WriteValue(new Uri("http://test.test"));
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got String.", a.Message);
            Assert.AreEqual("http://test.test", a.ValidationError.Value);
            a = null;

            validatingWriter.WriteValue(TimeSpan.FromMinutes(1.0));
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got String.", a.Message);
            Assert.AreEqual("00:01:00", a.ValidationError.Value);
            a = null;

            validatingWriter.WriteValue(new Guid("3D1A74E8-0B2D-43F2-9D55-2E9DD4A1598E"));
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got String.", a.Message);
            Assert.AreEqual("3d1a74e8-0b2d-43f2-9d55-2e9dd4a1598e", a.ValidationError.Value);
            a = null;

            validatingWriter.WriteValue(Encoding.UTF8.GetBytes("Hello world."));
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got String.", a.Message);
            Assert.AreEqual("SGVsbG8gd29ybGQu", a.ValidationError.Value);
            a = null;

            validatingWriter.WriteValue(new DateTimeOffset(2000, 12, 2, 5, 6, 2, TimeSpan.Zero));
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got String.", a.Message);
            Assert.AreEqual("2000-12-02T05:06:02+00:00", a.ValidationError.Value);
            a = null;

            validatingWriter.WriteValue(new DateTime(2000, 12, 2, 5, 6, 2, DateTimeKind.Utc));
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got String.", a.Message);
            Assert.AreEqual("2000-12-02T05:06:02Z", a.ValidationError.Value);
            a = null;

            validatingWriter.WriteEndArray();
            Assert.IsNull(a);

            validatingWriter.Flush();

            string json = sw.ToString();
            Console.WriteLine(json);

            StringAssert.AreEqual(@"[
  ""string"",
  null,
  ""e"",
  true,
  null,
  null,
  undefined,
  12,
  12,
  12,
  12,
  12,
  12,
  12,
  12,
  1.1,
  1.1,
  1.1,
  ""http://test.test"",
  ""00:01:00"",
  ""3d1a74e8-0b2d-43f2-9d55-2e9dd4a1598e"",
  ""SGVsbG8gd29ybGQu"",
  ""2000-12-02T05:06:02+00:00"",
  ""2000-12-02T05:06:02Z""
]", json);
        }

        [Test]
        public void WriteStartConstructor()
        {
            JSchema schema = new JSchema();
            schema.Type = JSchemaType.Array;
            schema.Items.Add(new JSchema
            {
                Type = JSchemaType.Integer
            });

            SchemaValidationEventArgs a = null;

            StringWriter sw = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);
            JSchemaValidatingWriter validatingWriter = new JSchemaValidatingWriter(writer);
            validatingWriter.Schema = schema;
            validatingWriter.ValidationEventHandler += (sender, args) =>
            {
                a = args;
            };

            validatingWriter.WriteStartConstructor("Test");

            Assert.IsNotNull(a);
            StringAssert.AreEqual(@"Invalid type. Expected Array but got Constructor.", a.Message);
            Assert.AreEqual(null, a.ValidationError.Value);
            a = null;

            validatingWriter.WriteValue(1);
            Assert.IsNull(a);

            validatingWriter.WriteValue('e');
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got String.", a.Message);
            Assert.AreEqual("e", a.ValidationError.Value);
            a = null;

            validatingWriter.WriteComment("comment!");
            Assert.IsNull(a);

            validatingWriter.WriteEndConstructor();
            Assert.IsNull(a);
        }
    }
}
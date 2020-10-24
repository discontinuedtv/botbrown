namespace BotBrown.Configuration
{
    using Newtonsoft.Json;
    using NUnit.Framework;
    using System;
    using System.IO;
    using System.Linq;

    [TestFixture]
    public class BirthdayConfigurationTest
    {

        [Test]
        public void BirthdayConfiguration_ShouldSerializeCorrectly()
        {
            var conf = new BirthdaysConfiguration();
            conf.AddBirthday(new DateTime(2000, 12, 14), "asdd");

            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            using StringWriter sw = new StringWriter();
            using JsonWriter writer = new JsonTextWriter(sw);

            serializer.Serialize(writer, conf);

            Assert.That(sw.ToString(), Is.EqualTo("{\"Birthdays\":[{\"Key\":{\"Day\":14,\"Month\":12},\"Value\":[{\"Day\":\"2000-12-14T00:00:00\",\"UserId\":\"asdd\",\"Gratulated\":[]}]}]}"));
        }

        [Test]
        public void BirthdayConfiguration_ShouldDeserializeCorrectly()
        {
            var confString = "{\"Birthdays\":[{\"Key\":{\"Day\":14,\"Month\":12},\"Value\":[{\"Day\":\"2000-12-14T00:00:00\",\"UserId\":\"asdd\",\"Gratulated\":[]}]}]}";

            using TextReader reader = new StringReader(confString);
            string serialzedConfiguration = reader.ReadToEnd();
            var configuration = JsonConvert.DeserializeObject<BirthdaysConfiguration>(serialzedConfiguration);

            Assert.That(configuration.Birthdays.Count, Is.EqualTo(1));

            var birthday = configuration.Birthdays.Single();
            Assert.That(birthday.Key.Day, Is.EqualTo(14));
            Assert.That(birthday.Key.Month, Is.EqualTo(12));
            Assert.That(birthday.Value.Count, Is.EqualTo(1));
            Assert.That(birthday.Value[0].Day, Is.EqualTo(new DateTime(2000, 12, 14)));
            Assert.That(birthday.Value[0].Gratulated, Is.Empty);
            Assert.That(birthday.Value[0].UserId, Is.EqualTo("asdd"));
        }
    }
}

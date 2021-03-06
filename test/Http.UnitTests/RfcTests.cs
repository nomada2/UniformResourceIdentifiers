﻿using System.Collections.Generic;
using Nito.UniformResourceIdentifiers;
using Nito.UniformResourceIdentifiers.Implementation;
using Xunit;

namespace Http.UnitTests
{
    public class RfcTests
    {
        private HttpUniformResourceIdentifier BaseUri = new HttpUniformResourceIdentifierBuilder().WithHost("a").WithPathSegments("b", "c", "d;p").WithQuery("q").Build();
        private HttpsUniformResourceIdentifier HttpsBaseUri = new HttpsUniformResourceIdentifierBuilder().WithHost("a").WithPathSegments("b", "c", "d;p").WithQuery("q").Build();

        [Fact]
        public void ReferenceResolutionExamplesBaseUri()
        {
            // See 5.4
            Assert.Equal("http://a/b/c/d;p?q", BaseUri.ToString());
            Assert.Equal("https://a/b/c/d;p?q", HttpsBaseUri.ToString());
        }

        [Fact]
        public void ReferenceResolutionNormalExamples_SpecifyingScheme()
        {
            // See 5.4.1, first example only
            var referenceUri = (GenericUniformResourceIdentifier)Factories.Create("g", null, null, null, new [] { "h" }, null, null);
            Assert.Equal("g:h", referenceUri.ToString());

            var result = BaseUri.Resolve(referenceUri);
            Assert.Equal("g:h", result.ToString());

            var httpsResult = HttpsBaseUri.Resolve(referenceUri);
            Assert.Equal("g:h", httpsResult.ToString());
        }

        [Theory]
        [InlineData("http://a/b/c/g", "g", null, new[] { "g" }, null, null)]
        [InlineData("http://a/b/c/g", "./g", null, new[] { ".", "g" }, null, null)]
        [InlineData("http://a/b/c/g/", "g/", null, new[] { "g", "" }, null, null)]
        [InlineData("http://a/g", "/g", null, new[] { "", "g" }, null, null)]
        [InlineData("http://g", "//g", "g", new[] { "" }, null, null)]
        [InlineData("http://a/b/c/d;p?y", "?y", null, new[] { "" }, "y", null)]
        [InlineData("http://a/b/c/g?y", "g?y", null, new[] { "g" }, "y", null)]
        [InlineData("http://a/b/c/d;p?q#s", "#s", null, new[] { "" }, null, "s")]
        [InlineData("http://a/b/c/g#s", "g#s", null, new[] { "g" }, null, "s")]
        [InlineData("http://a/b/c/g?y#s", "g?y#s", null, new[] { "g" }, "y", "s")]
        [InlineData("http://a/b/c/;x", ";x", null, new[] { ";x" }, null, null)]
        [InlineData("http://a/b/c/g;x", "g;x", null, new[] { "g;x" }, null, null)]
        [InlineData("http://a/b/c/g;x?y#s", "g;x?y#s", null, new[] { "g;x" }, "y", "s")]
        [InlineData("http://a/b/c/d;p?q", "", null, new[] { "" }, null, null)]
        [InlineData("http://a/b/c/", ".", null, new[] { "." }, null, null)]
        [InlineData("http://a/b/c/", "./", null, new[] { ".", "" }, null, null)]
        [InlineData("http://a/b/", "..", null, new[] { ".." }, null, null)]
        [InlineData("http://a/b/", "../", null, new[] { "..", "" }, null, null)]
        [InlineData("http://a/b/g", "../g", null, new[] { "..", "g" }, null, null)]
        [InlineData("http://a/", "../..", null, new[] { "..", ".." }, null, null)]
        [InlineData("http://a/", "../../", null, new[] { "..", "..", "" }, null, null)]
        [InlineData("http://a/g", "../../g", null, new[] { "..", "..", "g" }, null, null)]
        public void ReferenceResolutionNormalExamples(string expectedUrl, string expectedReferenceUrl, string host, IEnumerable<string> path, string query, string fragment)
        {
            // See 5.4.1
            var referenceUri = (RelativeReference)Factories.Create(null, null, host, null, path, query, fragment);
            Assert.Equal(expectedReferenceUrl, referenceUri.ToString());

            var result = BaseUri.Resolve(referenceUri);
            Assert.Equal(expectedUrl, result.ToString());
        }

        [Theory]
        [InlineData("https://a/b/c/g", "g", null, new[] { "g" }, null, null)]
        [InlineData("https://a/b/c/g", "./g", null, new[] { ".", "g" }, null, null)]
        [InlineData("https://a/b/c/g/", "g/", null, new[] { "g", "" }, null, null)]
        [InlineData("https://a/g", "/g", null, new[] { "", "g" }, null, null)]
        [InlineData("https://g", "//g", "g", new[] { "" }, null, null)]
        [InlineData("https://a/b/c/d;p?y", "?y", null, new[] { "" }, "y", null)]
        [InlineData("https://a/b/c/g?y", "g?y", null, new[] { "g" }, "y", null)]
        [InlineData("https://a/b/c/d;p?q#s", "#s", null, new[] { "" }, null, "s")]
        [InlineData("https://a/b/c/g#s", "g#s", null, new[] { "g" }, null, "s")]
        [InlineData("https://a/b/c/g?y#s", "g?y#s", null, new[] { "g" }, "y", "s")]
        [InlineData("https://a/b/c/;x", ";x", null, new[] { ";x" }, null, null)]
        [InlineData("https://a/b/c/g;x", "g;x", null, new[] { "g;x" }, null, null)]
        [InlineData("https://a/b/c/g;x?y#s", "g;x?y#s", null, new[] { "g;x" }, "y", "s")]
        [InlineData("https://a/b/c/d;p?q", "", null, new[] { "" }, null, null)]
        [InlineData("https://a/b/c/", ".", null, new[] { "." }, null, null)]
        [InlineData("https://a/b/c/", "./", null, new[] { ".", "" }, null, null)]
        [InlineData("https://a/b/", "..", null, new[] { ".." }, null, null)]
        [InlineData("https://a/b/", "../", null, new[] { "..", "" }, null, null)]
        [InlineData("https://a/b/g", "../g", null, new[] { "..", "g" }, null, null)]
        [InlineData("https://a/", "../..", null, new[] { "..", ".." }, null, null)]
        [InlineData("https://a/", "../../", null, new[] { "..", "..", "" }, null, null)]
        [InlineData("https://a/g", "../../g", null, new[] { "..", "..", "g" }, null, null)]
        public void HttpsReferenceResolutionNormalExamples(string expectedUrl, string expectedReferenceUrl, string host, IEnumerable<string> path, string query, string fragment)
        {
            // See 5.4.1
            var referenceUri = (RelativeReference)Factories.Create(null, null, host, null, path, query, fragment);
            Assert.Equal(expectedReferenceUrl, referenceUri.ToString());

            var result = HttpsBaseUri.Resolve(referenceUri);
            Assert.Equal(expectedUrl, result.ToString());
        }

        [Theory]
        [InlineData("http://a/g", "../../../g", null, new[] { "..", "..", "..", "g" }, null, null)]
        [InlineData("http://a/g", "../../../../g", null, new[] { "..", "..", "..", "..", "g" }, null, null)]
        [InlineData("http://a/g", "/./g", null, new[] { "", ".", "g" }, null, null)]
        [InlineData("http://a/g", "/../g", null, new[] { "", "..", "g" }, null, null)]
        [InlineData("http://a/b/c/g.", "g.", null, new[] { "g." }, null, null)]
        [InlineData("http://a/b/c/.g", ".g", null, new[] { ".g" }, null, null)]
        [InlineData("http://a/b/c/g..", "g..", null, new[] { "g.." }, null, null)]
        [InlineData("http://a/b/c/..g", "..g", null, new[] { "..g" }, null, null)]
        [InlineData("http://a/b/g", "./../g", null, new[] { ".", "..", "g" }, null, null)]
        [InlineData("http://a/b/c/g/", "./g/.", null, new[] { ".", "g", "." }, null, null)]
        [InlineData("http://a/b/c/g/h", "g/./h", null, new[] { "g", ".", "h" }, null, null)]
        [InlineData("http://a/b/c/h", "g/../h", null, new[] { "g", "..", "h" }, null, null)]
        [InlineData("http://a/b/c/g;x=1/y", "g;x=1/./y", null, new[] { "g;x=1", ".", "y" }, null, null)]
        [InlineData("http://a/b/c/y", "g;x=1/../y", null, new[] { "g;x=1", "..", "y" }, null, null)]
        [InlineData("http://a/b/c/g?y/./x", "g?y/./x", null, new[] { "g" }, "y/./x", null)]
        [InlineData("http://a/b/c/g?y/../x", "g?y/../x", null, new[] { "g" }, "y/../x", null)]
        [InlineData("http://a/b/c/g#s/./x", "g#s/./x", null, new[] { "g" }, null, "s/./x")]
        [InlineData("http://a/b/c/g#s/../x", "g#s/../x", null, new[] { "g" }, null, "s/../x")]
        public void ReferenceResolutionAbnormalExamples(string expectedUrl, string expectedReferenceUrl, string host, IEnumerable<string> path, string query, string fragment)
        {
            // See 5.4.2
            var referenceUri = (RelativeReference)Factories.Create(null, null, host, null, path, query, fragment);
            Assert.Equal(expectedReferenceUrl, referenceUri.ToString());

            var result = BaseUri.Resolve(referenceUri);
            Assert.Equal(expectedUrl, result.ToString());
        }

        [Theory]
        [InlineData("https://a/g", "../../../g", null, new[] { "..", "..", "..", "g" }, null, null)]
        [InlineData("https://a/g", "../../../../g", null, new[] { "..", "..", "..", "..", "g" }, null, null)]
        [InlineData("https://a/g", "/./g", null, new[] { "", ".", "g" }, null, null)]
        [InlineData("https://a/g", "/../g", null, new[] { "", "..", "g" }, null, null)]
        [InlineData("https://a/b/c/g.", "g.", null, new[] { "g." }, null, null)]
        [InlineData("https://a/b/c/.g", ".g", null, new[] { ".g" }, null, null)]
        [InlineData("https://a/b/c/g..", "g..", null, new[] { "g.." }, null, null)]
        [InlineData("https://a/b/c/..g", "..g", null, new[] { "..g" }, null, null)]
        [InlineData("https://a/b/g", "./../g", null, new[] { ".", "..", "g" }, null, null)]
        [InlineData("https://a/b/c/g/", "./g/.", null, new[] { ".", "g", "." }, null, null)]
        [InlineData("https://a/b/c/g/h", "g/./h", null, new[] { "g", ".", "h" }, null, null)]
        [InlineData("https://a/b/c/h", "g/../h", null, new[] { "g", "..", "h" }, null, null)]
        [InlineData("https://a/b/c/g;x=1/y", "g;x=1/./y", null, new[] { "g;x=1", ".", "y" }, null, null)]
        [InlineData("https://a/b/c/y", "g;x=1/../y", null, new[] { "g;x=1", "..", "y" }, null, null)]
        [InlineData("https://a/b/c/g?y/./x", "g?y/./x", null, new[] { "g" }, "y/./x", null)]
        [InlineData("https://a/b/c/g?y/../x", "g?y/../x", null, new[] { "g" }, "y/../x", null)]
        [InlineData("https://a/b/c/g#s/./x", "g#s/./x", null, new[] { "g" }, null, "s/./x")]
        [InlineData("https://a/b/c/g#s/../x", "g#s/../x", null, new[] { "g" }, null, "s/../x")]
        public void HttpsReferenceResolutionAbnormalExamples(string expectedUrl, string expectedReferenceUrl, string host, IEnumerable<string> path, string query, string fragment)
        {
            // See 5.4.2
            var referenceUri = (RelativeReference)Factories.Create(null, null, host, null, path, query, fragment);
            Assert.Equal(expectedReferenceUrl, referenceUri.ToString());

            var result = HttpsBaseUri.Resolve(referenceUri);
            Assert.Equal(expectedUrl, result.ToString());
        }
    }
}

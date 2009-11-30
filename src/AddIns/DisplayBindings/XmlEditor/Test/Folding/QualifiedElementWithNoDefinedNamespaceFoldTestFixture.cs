﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Folding
{
	[TestFixture]
	public class QualifiedElementWithNoDefinedNamespaceFoldTestFixture
	{
		XmlFoldParser parser;
		ICompilationUnit unit;
		DefaultXmlFileExtensions extensions;
		DefaultProjectContent projectContent;
		MockTextBuffer textBuffer;
		XmlEditorOptions options;
		
		[SetUp]
		public void Init()
		{
			string xml = 
				"<t:root>\r\n" +
				"</t:root>";
			
			projectContent = new DefaultProjectContent();
			textBuffer = new MockTextBuffer(xml);
			extensions = new DefaultXmlFileExtensions(null);
			options = new XmlEditorOptions(new Properties());
			MockParserService parserService = new MockParserService();
			
			parser = new XmlFoldParser(extensions, options, parserService);
			unit = parser.Parse(projectContent, @"a.xml", textBuffer);
		}
		
		[Test]
		public void FoldNameContainsFullyQualifiedElementName()
		{
			Assert.AreEqual("<t:root>", unit.FoldingRegions[0].Name);
		}
		
		[Test]
		public void FoldRegionContainsRootElement()
		{
			int beginLine = 1;
			int beginColumn = 1;
			int endLine = 2;
			int endColumn = 10;
			DomRegion expectedRegion = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			Assert.AreEqual(expectedRegion, unit.FoldingRegions[0].Region);
		}
	}
}

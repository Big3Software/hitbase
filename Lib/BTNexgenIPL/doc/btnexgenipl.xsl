<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<HTML>
			<HEAD>
				<TITLE>| Binary Technologies.com - API Documentation |</TITLE>
				<LINK REL="stylesheet" HREF="btdoc.css" TYPE="text/css"/>
			</HEAD>
			<BODY BGCOLOR="#FFFFFF" TEXT="#000000" LINK="#666666" VLINK="#990000">
				<a name="top"></a>
				<xsl:apply-templates select="btdoc"/>
			</BODY>
		</HTML>
	</xsl:template>

	<xsl:template match="btdoc">
		<xsl:apply-templates select="classsynopsis"/>
		<p></p>
		<hr size="1" noshade="" align="center"></hr>
		<span class="link">NexgenIPL (c) 1999 - 2003 <a href="http://www.binary-technologies.com" target="_blank">Binary Technologies</a></span>
		<p></p>
	</xsl:template>

	<xsl:template match="classsynopsis">
		<p class="h1"><xsl:value-of select="ooclass/classname"/></p>
		<xsl:call-template name="create-alphabeticalindex"/>
		<xsl:for-each select="methodsynopsis">
			<xsl:sort select="methodname"/>
			<xsl:call-template name="methodsynopsis"/>
		</xsl:for-each>
		<xsl:call-template name="create-methodindex"/>
	</xsl:template>

	<xsl:template name="methodsynopsis">
		<p class="h2">
		<xsl:value-of select="methodname"/>
		<a><xsl:attribute name="name"><xsl:value-of select="methodname"/></xsl:attribute></a>
		</p>

		<!-- Build the 'method body' -->
		<p class="method">
		<xsl:value-of select="modifier"/>
		<xsl:text> </xsl:text>
		<xsl:value-of select="type"/>
		<xsl:text> </xsl:text>
		<xsl:value-of select="methodname"/>
		<xsl:text>(</xsl:text>
		<xsl:for-each select="methodparam">
			<xsl:value-of select="type"/>
			<xsl:text> </xsl:text>
			<xsl:value-of select="parameter"/>
			<xsl:if test="count(initializer) != 0">
				<xsl:text> /* = </xsl:text>
				<xsl:value-of select="initializer"/>
				<xsl:text> */</xsl:text>
			</xsl:if>
			<xsl:if test="position() != last()">
				<xsl:text>, </xsl:text>
			</xsl:if>
		</xsl:for-each>
		<xsl:text>)</xsl:text>
		</p>
		<p class="desc"><xsl:value-of disable-output-escaping="yes" select="desc"/></p>
		<xsl:if test="count(methodparam) != 0">
			<p class="h3">Parameters</p>
			<xsl:apply-templates select="methodparam"/>
		</xsl:if>
		<p class="h3">Return Value</p>
		<p class="desc"><xsl:value-of disable-output-escaping="yes" select="retvaldesc"/></p>
		<!-- Generate top and index links -->
		<a href="#top" class="link">Top</a>
		<xsl:text> </xsl:text>
		<a href="#index" class="link">Index</a>
		<xsl:if test="position() != last()">
			<hr size="1" noshade="" align="center"></hr>
		</xsl:if>
	</xsl:template>

	<xsl:template match="methodparam">
		<xsl:for-each select=".">
			<p class="param"><xsl:value-of select="parameter"/></p>
			<blockquote>
				<p class="desc"><xsl:value-of disable-output-escaping="yes" select="desc"/></p>
				<xsl:apply-templates select="enumlist"/>
			</blockquote>
		</xsl:for-each>
	</xsl:template>

	<xsl:template match="enumlist">
		<ul>
		<xsl:apply-templates select="enum"/>
		</ul>
	</xsl:template>

	<xsl:template match="enum">
		<li class="list"><span class="Text"><xsl:value-of select="."/></span></li>
	</xsl:template>

	<xsl:template name="create-alphabeticalindex">
		<xsl:for-each select="methodsynopsis">
			<xsl:sort select="methodname" data-type="text"/>
			<xsl:variable name="c" select="substring(methodname,1,1)"/>
			<xsl:if test="parent::node()/child::methodsynopsis[substring(methodname,1,1) = $c][1] = current()">
				<a>
				<xsl:attribute name="href">#<xsl:value-of select="methodname"/></xsl:attribute>
				<xsl:value-of select="$c"/>
				</a>
				<xsl:if test="not(position()=last())">
					<xsl:text>&#160;</xsl:text>
				</xsl:if>
			</xsl:if>
		</xsl:for-each>
		<xsl:text>&#160;</xsl:text>
		<a href="#index">[Index]</a>
		<hr size="1" noshade="" align="center"></hr>
	</xsl:template>

	<xsl:template name="create-methodindex">
		<hr size="1" noshade="" align="center"></hr>
		<a name="index"></a>
		<h2>Index</h2>
		<xsl:for-each select="methodsynopsis">
			<xsl:sort select="methodname" data-type="text"/>
			<xsl:variable name="initiale" select="methodname"/>
			<xsl:if test="not(preceding::node()[methodname=$initiale])">
				<a>
				<xsl:attribute name="href">#<xsl:value-of select="methodname"/></xsl:attribute>
				<xsl:value-of select="methodname"/>
				</a>
				<xsl:if test="not(position()=last())">
					<br/>
				</xsl:if>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>

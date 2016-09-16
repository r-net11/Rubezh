<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:wix="http://wixtoolset.org/schemas/v4/wxs">
	<xsl:output method="xml" indent="yes" />
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>
	<xsl:key name="service-search" match="wix:Component[contains(wix:File/@Source, '.pdb')]" use="@Id" />
	<xsl:key name="service-search" match="wix:Component[contains(wix:File/@Source, '.vshost.exe')]" use="@Id" />
	<xsl:key name="service-search" match="wix:Component[contains(wix:File/@Source, '.exe.config')]" use="@Id" />
	<xsl:key name="service-search" match="wix:Component[contains(wix:File/@Source, '.xml')]" use="@Id" />
	<xsl:key name="service-search" match="wix:Component[contains(wix:File/@Source, '.lic')]" use="@Id" />
	<xsl:key name="service-search" match="wix:Component[contains(wix:File/@Source, 'GenerateKeyInstaller.exe')]" use="@Id" />
	<!--
	<xsl:key name="service-search" match="wix:Component[contains(wix:File/@Source, '.dll.config')]" use="@Id" />

	<xsl:key name="service-search" match="wix:Component[contains(wix:File/@Source, 'app.confing')]" use="@Id" />-->

	<xsl:template match="wix:Component[key('service-search', @Id)]" />
	<xsl:template match="wix:ComponentRef[key('service-search', @Id)]" />
	<!--<xsl:key name="service-search" match="wix:Component[wix:File/@Source = '$(var.GenerateKeyApplication.TargetDir)\GenerateKeyApplication.exe']" use="@Id" />-->



</xsl:stylesheet>

<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:wix="http://schemas.microsoft.com/wix/2006/wi"
  xmlns="http://schemas.microsoft.com/wix/2006/wi"
  exclude-result-prefixes="wix"
    >
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="wix:Wix">

    <!--Выбираем иконки-->
    <xsl:variable name="reportTemplates" select="wix:Fragment/wix:DirectoryRef/wix:Directory/wix:Component[substring(wix:File/@Source, string-length(wix:File/@Source) - 2) = 'rpt']"/>

    <Wix>
      <Fragment>
        <DirectoryRef Id="ReportTemplates">
          <xsl:apply-templates select="$reportTemplates"/>
        </DirectoryRef>
      </Fragment>

      <Fragment>
        <ComponentGroup Id="ReportTemplates">
          <xsl:for-each select="$reportTemplates">
            <ComponentRef Id="{@Id}"/>
          </xsl:for-each>
        </ComponentGroup>
      </Fragment>

    </Wix>

  </xsl:template>


  <xsl:template match="wix:Component">
    <Component Id="{@Id}" Guid="{@Guid}">
      <File Id="{wix:File/@Id}" Source="{concat('..\..\Configuration\ReportTemplates', substring(wix:File/@Source, 10))}" />
    </Component>
  </xsl:template>

</xsl:stylesheet>

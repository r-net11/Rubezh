<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:wix="http://schemas.microsoft.com/wix/2006/wi"
  xmlns="http://schemas.microsoft.com/wix/2006/wi"
  exclude-result-prefixes="wix"
    >
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="wix:Wix">

    <!--Выбираем иконки-->
    <xsl:variable name="sounds" select="wix:Fragment/wix:DirectoryRef/wix:Directory/wix:Component[substring(wix:File/@Source, string-length(wix:File/@Source) - 2) = 'wav']"/>

    <Wix>
      <Fragment>
        <DirectoryRef Id="Output.Sounds">
          <xsl:apply-templates select="$sounds"/>
        </DirectoryRef>
      </Fragment>

      <Fragment>
        <ComponentGroup Id="Output.Sounds">
          <xsl:for-each select="$sounds">
            <ComponentRef Id="{@Id}"/>
          </xsl:for-each>
        </ComponentGroup>
      </Fragment>

    </Wix>

  </xsl:template>


  <xsl:template match="wix:Component">
    <Component Id="{@Id}" Guid="{@Guid}">
      <File Id="{wix:File/@Id}" Source="{concat('..\..\source\Configuration\Sounds', substring(wix:File/@Source, 10))}" />
    </Component>
  </xsl:template>

</xsl:stylesheet>

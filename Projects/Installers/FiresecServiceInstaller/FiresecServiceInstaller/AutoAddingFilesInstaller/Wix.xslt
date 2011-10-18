<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:wix="http://schemas.microsoft.com/wix/2006/wi"
  xmlns="http://schemas.microsoft.com/wix/2006/wi"
  exclude-result-prefixes="wix"
    >
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="wix:Wix">

    <!--Выбираем *.config за исключением vshost и App.config-->
    <xsl:variable name="content" select="wix:Fragment/wix:DirectoryRef/wix:Directory/wix:Component[substring(wix:File/@Source, string-length(wix:File/@Source) - 5) = 'config' and not(contains(wix:File/@Source, 'vshost')) and not(substring(wix:File/@Source, string-length(wix:File/@Source) - 9) = 'App.config')]"/>

    <!--Выбираем *.exe & *.dll за исключением vshost-->
    <xsl:variable name="binaries" select="wix:Fragment/wix:DirectoryRef/wix:Directory/wix:Component[substring(wix:File/@Source, string-length(wix:File/@Source) - 2) = 'dll' or substring(wix:File/@Source, string-length(wix:File/@Source) - 2) = 'exe' and not(contains(wix:File/@Source, 'vshost'))]"/>

    <!--Выбираем иконки-->
    <xsl:variable name="icons" select="wix:Fragment/wix:DirectoryRef/wix:Directory/wix:Component[substring(wix:File/@Source, string-length(wix:File/@Source) - 2) = 'ico']"/>

    <Wix>
      <Fragment>
        <DirectoryRef Id="Output.Content">
          <xsl:apply-templates select="$content"/>
        </DirectoryRef>

        <DirectoryRef Id="Output.Binaries">
          <xsl:apply-templates select="$binaries"/>
        </DirectoryRef>

        <DirectoryRef Id="Output.Icons">
          <xsl:apply-templates select="$icons"/>
        </DirectoryRef>
      </Fragment>

      <Fragment>
        <ComponentGroup Id="Output.Content">
          <xsl:for-each select="$content">
            <ComponentRef Id="{@Id}"/>
          </xsl:for-each>
        </ComponentGroup>
        <ComponentGroup Id="Output.Binaries">
          <xsl:for-each select="$binaries">
            <ComponentRef Id="{@Id}"/>
          </xsl:for-each>
        </ComponentGroup>
        <ComponentGroup Id="Output.Icons">
          <xsl:for-each select="$icons">
            <ComponentRef Id="{@Id}"/>
          </xsl:for-each>
        </ComponentGroup>
        
      </Fragment>

    </Wix>

  </xsl:template>


  <xsl:template match="wix:Component">
    <Component Id="{@Id}" Guid="{@Guid}">
      <File Id="{wix:File/@Id}" Source="{concat('..\..\source', substring(wix:File/@Source, 10))}" />
    </Component>
  </xsl:template>

</xsl:stylesheet>

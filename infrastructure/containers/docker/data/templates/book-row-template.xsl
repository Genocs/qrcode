<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format">
  
  <!-- Template for each book row -->
  <xsl:template match="/PdfPrinter/Books/BookList/Book">
    <fo:table-row display-align="center" font-size="6pt" height="0.5cm">
      <fo:table-cell border="0.1pt solid black">
        <fo:block>
          <xsl:value-of select="Title"/>
        </fo:block>
      </fo:table-cell>
    </fo:table-row>
  </xsl:template>
  
</xsl:stylesheet>

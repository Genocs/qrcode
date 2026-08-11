<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format">
	
	<!--Import static content of template-->
	<xsl:import href="header.xsl" />
	<xsl:import href="footer.xsl" />
	<xsl:import href="book-row-template.xsl" />
	
	<xsl:output encoding="utf-8" indent="yes" method="xml" omit-xml-declaration="yes" />
	
	<xsl:template match="/">
		
		<fo:root>
			<fo:layout-master-set>
				<!-- Master for the first page -->
				<fo:simple-page-master master-name="first-page-master" page-height="297mm" page-width="210mm" margin="10mm">
					<fo:region-before extent="32mm" region-name="header-first-region" overflow="visible" />
					<fo:region-body margin-top="50mm" margin-bottom="16mm" />
					<fo:region-after extent="14mm" region-name="footer-region"/>
				</fo:simple-page-master>
				
				<!-- Master for subsequent pages -->
				<fo:simple-page-master master-name="other-pages-master" page-height="297mm" page-width="210mm" margin="10mm">
					<fo:region-before extent="22mm" region-name="header-other-region" overflow="visible" />
					<fo:region-body margin-top="22mm" margin-bottom="16mm" />
					<fo:region-after extent="14mm" region-name="footer-region"/>
				</fo:simple-page-master>
				
				<fo:page-sequence-master master-name="document-master">
					<fo:repeatable-page-master-alternatives maximum-repeats="no-limit">
						<fo:conditional-page-master-reference master-reference="first-page-master" page-position="first" />
						<fo:conditional-page-master-reference master-reference="other-pages-master" page-position="rest" />
					</fo:repeatable-page-master-alternatives>
				</fo:page-sequence-master>
			</fo:layout-master-set>
			
			<fo:page-sequence initial-page-number="2" master-reference="document-master">
				<!-- First page header -->
				<xsl:call-template name="header-first-page-template" />
				
				<!-- Other pages header -->
				<xsl:call-template name="header-other-pages-template" />
				
				<!--footer-->
				<xsl:call-template name="footer-template" />
				
				<!-- Document content start here -->
				<fo:flow flow-name="xsl-region-body" font-family="Nunito" font-size="10pt">
					
					<fo:block text-align="left">
						<fo:table border-spacing="2pt"
							table-layout="fixed"
							table-omit-footer-at-break="true">
							
							<!-- Column definition -->
							<fo:table-column column-width="190mm"/>
							
							<!-- Table header -->
							<fo:table-header>
								<fo:table-row background-color="rgb(205, 205, 205)"
									display-align="center" 
									height="0.5cm"
									text-align="center">
									<fo:table-cell padding-top="4pt"
										padding-left="1.1pt"
										padding-bottom="3pt">
										This is the Table Header
										<fo:block font-weight="bold" text-align="left">
											This is the Table Header
										</fo:block>
									</fo:table-cell>
								</fo:table-row>
							</fo:table-header>
							
							<!-- Table Footer (with calculated column) -->
							<fo:table-footer>
								<fo:table-row font-size="7pt"
									height="0.5cm"
									display-align="center"
									text-align="center"
									background-color="rgb(120, 120, 120)">
									<fo:table-cell padding-top="4pt"
										padding-bottom="3pt">
										<fo:block font-weight="bold" text-align="left">
											This is the Table Footer
										</fo:block>
									</fo:table-cell>
								</fo:table-row>
							</fo:table-footer>
							
							<!-- Table Body -->
							<fo:table-body>
								<!-- Read Data from model-->
								<xsl:for-each select="/PdfPrinter/Books/BookList/Book">
									<xsl:apply-templates select="."/>
								</xsl:for-each>
							</fo:table-body>
						</fo:table>
					</fo:block>
					
				</fo:flow>
			</fo:page-sequence>
		</fo:root>
	</xsl:template>
	
</xsl:stylesheet>




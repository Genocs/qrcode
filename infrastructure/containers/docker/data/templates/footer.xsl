<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:fo="http://www.w3.org/1999/XSL/Format">
	<xsl:template name="footer-template">
		<fo:static-content flow-name="footer-region">
			<fo:block>
				<fo:table>
					<fo:table-column column-width="200mm"/>
					<fo:table-body start-indent="0pt">
						<fo:table-row>
							<fo:table-cell>
								<fo:block>
									<fo:table>
										<fo:table-column column-width="160mm"/>
										<fo:table-column column-width="30mm"/>
										<fo:table-body start-indent="0pt">
											<fo:table-row>
												<fo:table-cell number-columns-spanned="2" display-align="center">
													<fo:block text-align="center">
														<fo:leader leader-pattern="rule" rule-thickness="1pt" leader-length="100%" color="rgb(128,128,128)"/>
													</fo:block>
												</fo:table-cell>
											</fo:table-row>
											<fo:table-row>
												<fo:table-cell font-size="7pt">
													<fo:block text-align="left">
														This is the footer section. You can customize it as you like.
													</fo:block>
												</fo:table-cell>
												<fo:table-cell font-size="8pt" display-align="center">
													<fo:block text-align="right">
														<!-- <xsl:value-of select="/PdfPrinter/culture/label[@id='Page']/@text" />-->
														<fo:page-number/>
													</fo:block>
												</fo:table-cell>
											</fo:table-row>
										</fo:table-body>
									</fo:table>
								</fo:block>
							</fo:table-cell>
						</fo:table-row>
					</fo:table-body>
				</fo:table>
			</fo:block>
		</fo:static-content>
	</xsl:template>
</xsl:stylesheet>

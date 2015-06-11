// MyEdit.cpp : 实现文件
//

#include "stdafx.h"
#include "AccessControl.h"
#include "MyEdit.h"


// CMyEdit

IMPLEMENT_DYNAMIC(CMyEdit, CEdit)

CMyEdit::CMyEdit()
{

}

CMyEdit::~CMyEdit()
{
}


BEGIN_MESSAGE_MAP(CMyEdit, CEdit)

	ON_WM_CHAR(this, &this::OnChar)
END_MESSAGE_MAP()



// CMyEdit 消息处理程序

void CMyEdit::OnChar(UINT nChar, UINT nRepCnt, UINT nFlags)
{
	// TODO: 在此添加消息处理程序代码和/或调用默认值
	if(nChar <= '9' && nChar >= '0')//数字
		CEdit::OnChar(nChar, nRepCnt, nFlags);
	if(nChar >='a' && nChar <= 'f')//常用字
		CEdit::OnChar(nChar, nRepCnt, nFlags);
	if(nChar >='A' && nChar <= 'F')//常用字
		CEdit::OnChar(nChar, nRepCnt, nFlags); 
	if(nChar == ' ')//常用字
		CEdit::OnChar(nChar, nRepCnt, nFlags); 
	return;
}

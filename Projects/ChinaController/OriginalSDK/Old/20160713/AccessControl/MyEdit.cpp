// MyEdit.cpp : ʵ���ļ�
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



// CMyEdit ��Ϣ�������

void CMyEdit::OnChar(UINT nChar, UINT nRepCnt, UINT nFlags)
{
	// TODO: �ڴ������Ϣ�����������/�����Ĭ��ֵ
	if(nChar <= '9' && nChar >= '0')//����
		CEdit::OnChar(nChar, nRepCnt, nFlags);
	if(nChar >='a' && nChar <= 'f')//������
		CEdit::OnChar(nChar, nRepCnt, nFlags);
	if(nChar >='A' && nChar <= 'F')//������
		CEdit::OnChar(nChar, nRepCnt, nFlags); 
	if(nChar == ' ')//������
		CEdit::OnChar(nChar, nRepCnt, nFlags); 
	return;
}

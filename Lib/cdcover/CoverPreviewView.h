#pragma once


// CCoverPreviewView-Ansicht

class CCoverPreviewView : public CPreviewView
{
	DECLARE_DYNCREATE(CCoverPreviewView)

protected:
	CCoverPreviewView();           // Dynamische Erstellung verwendet geschützten Konstruktor
	virtual ~CCoverPreviewView();

public:
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif
	void OnFilePrint();

protected:
	DECLARE_MESSAGE_MAP()
};



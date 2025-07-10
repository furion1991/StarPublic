import { API } from '@/shared/api'

import type { OpenedCaseData, OpenFewCasesProps, OpenCaseProps } from './cases.types'

export const openCase = ({ userId, caseId }: OpenCaseProps) => {
  return API.post<OpenedCaseData>('/case/open', {
    userId,
    caseId
  })
}

export const openFewCases = ({ userId, caseId, quantity }: OpenFewCasesProps) => {
  return API.post<OpenedCaseData>('/cases/open', {
    userId,
    caseId,
    quantity
  })
}

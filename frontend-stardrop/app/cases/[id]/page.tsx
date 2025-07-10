import { CasePage } from '@/pages'

import type { Metadata } from 'next'

type Props = {
  params: Promise<{ id: string }>
}

export async function generateMetadata({ params }: Props): Promise<Metadata> {
  const id = (await params).id
  const url = `${process.env.NEXT_PUBLIC_API_BASE}/v1/cases/get/${id}`
  const caseData = await fetch(url).then((res) => res.json())

  return {
    title: `Кейс - ${caseData.name}`,
    description: `Кейс - ${caseData.name}`
  }
}

export default CasePage

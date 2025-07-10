import Image from 'next/image'
import Link from 'next/link'

import { Button } from '@/shared/ui'

import { useFavoriteCase } from '../../model/useFavoriteCase'

import classes from './FavoriteCase.module.scss'

type FavoriteCaseProps = {
  userId: string
}

export const FavoriteCase = ({ userId }: FavoriteCaseProps) => {
  const { data: caseData, isLoading: isLoading } = useFavoriteCase({ userId })

  if (isLoading || !caseData) return null

  return (
    <div className={classes.favoriteCase}>
      <p>Любимый кейс</p>

      <p>{caseData.name}</p>

      <Link href={`/cases/${caseData.id}`}>
        <Button type='button' borderRadius='medium'>
          Открыть
        </Button>
      </Link>

      <div className={classes.caseBg}>
        <Image src='/placeholders/favorite-case-back.png' width={180} height={207} alt='фон' />
      </div>

      <div className={classes.caseImage}>
        <Image src={caseData.image} width={256} height={144} alt={caseData.name} quality={100} />
      </div>
    </div>
  )
}

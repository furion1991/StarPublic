import Image from 'next/image'
import cn from 'classnames'

import classes from './IdCopier.module.scss'

type IdCopierProps = {
  id: string
  className?: string
}

export const IdCopier = ({ id, className }: IdCopierProps) => {
  const copyIdToClipboard = (id: string) => {
    navigator.clipboard.writeText(id)
  }

  return (
    <div className={cn(classes.idCopier, className)}>
      ID: <span>{id}</span>
      <button
        type='button'
        onClick={() => {
          copyIdToClipboard(id)
        }}
      >
        <Image src='/img/copy.png' width={12} height={12} alt='Копировать' />{' '}
      </button>
    </div>
  )
}
